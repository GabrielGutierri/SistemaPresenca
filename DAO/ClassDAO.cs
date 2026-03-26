using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.DAO
{
    public class ClassDAO : BaseDAO<ClassViewModel>
    {
        protected override SqlParameter[] CreateParameters(ClassViewModel model)
        {
            SqlParameter[] parameters = new SqlParameter[4];
            parameters[0] = new SqlParameter("id", model.ID);
            parameters[1] = new SqlParameter("conteudo", model.Content);
            parameters[2] = new SqlParameter("datahoraaula", model.ClassDateTime);
            parameters[3] = new SqlParameter("codmateria", model.SubjectCode);
            return parameters;
        }

        protected override ClassViewModel BuildModel(DataRow row, bool withJoin = false)
        {
            ClassViewModel classModel = new ClassViewModel()
            {
                ID = Convert.ToInt32(row["id"]),
                Content = row["conteudo"].ToString(),
                ClassDateTime = Convert.ToDateTime(row["datahoraaula"]),
                SubjectCode = Convert.ToInt32(row["codmateria"])
            };
            if (withJoin)
                classModel.SubjectName = row["descricao"].ToString();
            return classModel;
        }

        protected override void SetTable()
        {
            Table = "Aula";
        }

        public List<ClassViewModel> GetClassListing()
        {
            var table = HelperDAO.ExecuteProcSelect("spListagemAulas", null);
            List<ClassViewModel> list = new List<ClassViewModel>();
            foreach (DataRow row in table.Rows)
                list.Add(BuildModel(row));
            return list;
        }

        public ClassViewModel QueryByDate(DateTime attendanceTime)
        {
            DateTime targetTime;

            if (attendanceTime.Hour < 22)
                targetTime = new DateTime(attendanceTime.Year, attendanceTime.Month, attendanceTime.Day, 19, 15, 00, 00);
            else
                throw new Exception("Class time not consistent with the night period");

            SqlParameter[] p =
            {
                new SqlParameter("data", targetTime)
            };
            var table = HelperDAO.ExecuteProcSelect("spConsultaAulaPorData", p);
            foreach (DataRow dr in table.Rows)
            {
                return BuildModel(dr);
            }
            return null;
        }

        public List<ClassViewModel> AdvancedQuery(DateTime startDate, DateTime endDate, int subjectId)
        {
            SqlParameter[] p =
            {
                new SqlParameter("data_inicial", startDate),
                new SqlParameter("data_final", endDate),
                new SqlParameter("id_materia", subjectId)
            };

            string query = "SELECT * from fnConsultaAvancada (@data_inicial, @data_final, @id_materia)";
            var table = HelperDAO.ExecuteFunctionSelect(query, p);
            var list = new List<ClassViewModel>();
            foreach (DataRow dr in table.Rows)
            {
                list.Add(new ClassViewModel
                {
                    Content = dr["conteudo"].ToString(),
                    ClassDateTime = Convert.ToDateTime(dr["datahoraaula"]),
                    SubjectName = dr["materia"].ToString()
                });
            }
            return list;
        }
    }
}