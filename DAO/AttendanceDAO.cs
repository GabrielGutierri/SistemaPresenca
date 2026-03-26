using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.DAO
{
    public class AttendanceDAO : BaseDAO<AttendanceViewModel>
    {
        protected override SqlParameter[] CreateParameters(AttendanceViewModel model)
        {
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("id", model.ID);
            parameters[1] = new SqlParameter("codAluno", model.StudentCode);
            parameters[2] = new SqlParameter("codAula", model.ClassCode);
            parameters[3] = new SqlParameter("presente", model.AttendanceStatus);
            parameters[4] = new SqlParameter("dataHoraPresenca", model.AttendanceDateTime);
            return parameters;
        }

        public List<string> GetNewAttendances(int studentCode)
        {
            SqlParameter[] p = new SqlParameter[1];
            p[0] = new SqlParameter("codAluno", studentCode);
            var table = HelperDAO.ExecuteProcSelect("spConsultaDataPresenca", p);
            var dateList = new List<string>();
            foreach (DataRow dr in table.Rows)
            {
                dateList.Add(Convert.ToDateTime(dr["HorarioPresenca"]).ToString("dd/MM/yyyy HH:mm"));
            }
            return dateList;
        }

        protected override AttendanceViewModel BuildModel(DataRow row, bool withJoin = false)
        {
            AttendanceViewModel model = new AttendanceViewModel()
            {
                ID = Convert.ToInt32(row["id"]),
                StudentCode = Convert.ToInt32(row["codAluno"]),
                ClassCode = Convert.ToInt32(row["codAula"]),
                AttendanceStatus = row["presente"].ToString(),
                AttendanceDateTime = Convert.ToDateTime(row["HorarioPresenca"]),
                StudentName = row["nome"].ToString(),
                ClassDescription = row["Conteudo"].ToString(),
                SubjectDescription = row["descricao"].ToString()
            };
            return model;
        }

        protected override void SetTable()
        {
            Table = "Presenca";
        }

        public void FetchMongoDBData()
        {
            string conn = "mongodb://helix:H3l1xNG@191.233.28.24:27000/?authMechanism=SCRAM-SHA-1";
            var client = new MongoClient(conn);
            var db = client.GetDatabase("sth_helixiot");
            foreach (var student in new StudentDAO().GetListing())
            {
                if (student.BiometricId != 0)
                {
                    var entity = db.GetCollection<CommandsModel>($"sth_/_urn:ngsi-ld:aluno:{student.BiometricId.ToString("D3")}_Aluno");

                    // In the student entity, get the documents of the attendance commands
                    // Get the student's new attendance records, ordered by most recent
                    AttendanceDAO attendanceDAO = new AttendanceDAO();
                    List<string> attendanceDateList = attendanceDAO.GetNewAttendances(student.ID);
                    var docs = entity.Find(x => x.attrName == "presenca").ToList();
                    docs.Reverse();
                    foreach (var doc in docs)
                    {
                        if (attendanceDateList.Contains(doc.recvTime.ToString("dd/MM/yyyy HH:mm")))
                        {
                            break;
                        }
                        InsertAttendanceSQL(doc, student.ID);
                    }
                }
            }
        }

        private void InsertAttendanceSQL(CommandsModel doc, int studentId)
        {
            ClassViewModel classRecord = GetDayClass(doc.recvTime);
            if (classRecord != null)
            {
                AssignStudentAttendance(studentId, classRecord.ID, doc.attrValue, doc.recvTime);
            }
        }

        private bool CheckStudentAttendance(int studentCode, int classCode)
        {
            SqlParameter[] p = new SqlParameter[]
            {
                new SqlParameter("tabela", "presenca"),
                new SqlParameter("ordem", 1)
            };

            var table = HelperDAO.ExecuteProcSelect("spListagem", p);
            var studentAttendances = new List<int>();
            foreach (DataRow row in table.Rows)
            {
                if (Convert.ToInt32(row["CodAluno"]) == studentCode)
                {
                    int foundClassCode = Convert.ToInt32(row["CodAula"]);
                    studentAttendances.Add(foundClassCode);
                }
            }

            if (studentAttendances.Find(x => x == classCode) >= 1)
                return false;
            else
                return true;
        }

        private void AssignStudentAttendance(int studentId, int classId, string status, DateTime attendanceTime)
        {
            if (CheckStudentAttendance(studentId, classId))
                Insert(new AttendanceViewModel { StudentCode = studentId, ClassCode = classId, AttendanceStatus = status, AttendanceDateTime = attendanceTime });
        }

        private ClassViewModel GetDayClass(DateTime receivedTime)
        {
            ClassDAO classDAO = new ClassDAO();
            return classDAO.QueryByDate(receivedTime);
        }

        public List<AttendanceViewModel> AdvancedAttendanceQuery(int studentId, int classId)
        {
            SqlParameter[] p =
            {
                new SqlParameter("idAluno", studentId),
                new SqlParameter("idAula", classId)
            };

            var table = HelperDAO.ExecuteProcSelect("spConsultaPresencaParametrizada", p);
            var attendanceList = new List<AttendanceViewModel>();
            foreach (DataRow dr in table.Rows)
            {
                attendanceList.Add(BuildModel(dr));
            }
            var result = AddAbsentStudents(studentId, classId, attendanceList);

            return result;
        }

        private List<AttendanceViewModel> AddAbsentStudents(int studentId, int classId, List<AttendanceViewModel> attendanceList)
        {
            // Scenarios:
            // class filter with all students  -> mark absent students for the class
            // student filter with all classes -> mark student as absent for each class
            // student filter with specific class -> mark as absent if not found
            StudentDAO studentDAO = new StudentDAO();
            ClassDAO classDAO = new ClassDAO();
            SubjectDAO subjectDAO = new SubjectDAO();

            if (studentId == 0 && classId != 0)
            {
                foreach (var student in studentDAO.GetListing())
                {
                    if (attendanceList.Find(a => a.StudentCode == student.ID) == null)
                    {
                        attendanceList.Add(new AttendanceViewModel
                        {
                            StudentCode = student.ID,
                            ClassCode = classId,
                            AttendanceStatus = "Not Present",
                            StudentName = student.Name,
                            ClassDescription = classDAO.Query(classId).Content,
                            SubjectDescription = subjectDAO.Query(classDAO.Query(classId).SubjectCode).Description
                        });
                    }
                }
                return attendanceList;
            }
            if (studentId != 0 && classId == 0)
            {
                foreach (var classItem in classDAO.GetListing())
                {
                    if (attendanceList.Find(a => a.ClassCode == classItem.ID) == null)
                    {
                        attendanceList.Add(new AttendanceViewModel
                        {
                            StudentCode = studentId,
                            ClassCode = classItem.ID,
                            AttendanceStatus = "Not Present",
                            StudentName = studentDAO.Query(studentId).Name,
                            ClassDescription = classItem.Content,
                            SubjectDescription = subjectDAO.Query(classItem.SubjectCode).Description
                        });
                    }
                }
                return attendanceList;
            }
            if (studentId != 0 && classId != 0)
            {
                if (attendanceList.Find(a => a.StudentCode == studentId) == null)
                {
                    attendanceList.Add(new AttendanceViewModel
                    {
                        StudentCode = studentId,
                        ClassCode = classId,
                        AttendanceStatus = "Not Present",
                        StudentName = studentDAO.Query(studentId).Name,
                        ClassDescription = classDAO.Query(classId).Content,
                        SubjectDescription = subjectDAO.Query(classDAO.Query(classId).SubjectCode).Description
                    });
                }
                return attendanceList;
            }
            else
            {
                return attendanceList;
            }
        }

        public List<int> GetAttendanceAndSubjectCounts(int subjectId)
        {
            SqlParameter[] pClass =
            {
                new SqlParameter("id", subjectId)
            };

            SqlParameter[] pStudent =
            {
                new SqlParameter("id", subjectId)
            };

            var presentCount = HelperDAO.ExecuteProcSelect("spPegaQuantidadePresente", pClass);
            var classSubjectCount = HelperDAO.ExecuteProcSelect("spPegaQuantidadedeAulasMaterias", pStudent);

            var counts = new List<int>();
            counts.Add(Convert.ToInt32(presentCount.Rows[0][0]));
            counts.Add(Convert.ToInt32(classSubjectCount.Rows[0][0]));

            return counts;
        }
    }
}