using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.DAO
{
    public class SubjectDAO : BaseDAO<SubjectViewModel>
    {
        protected override SqlParameter[] CreateParameters(SubjectViewModel model)
        {
            SqlParameter[] parameters = new SqlParameter[4];
            parameters[0] = new SqlParameter("id", model.ID);
            parameters[1] = new SqlParameter("descricao", model.Description);
            parameters[2] = new SqlParameter("cargahoraria", model.CourseLoad);
            parameters[3] = new SqlParameter("codprofessor", model.TeacherCode);
            return parameters;
        }

        protected override SubjectViewModel BuildModel(DataRow row, bool withJoin = false)
        {
            SubjectViewModel model = new SubjectViewModel()
            {
                ID = Convert.ToInt32(row["id"]),
                Description = row["descricao"].ToString(),
                CourseLoad = Convert.ToDouble(row["cargahoraria"]),
                TeacherCode = Convert.ToInt32(row["codprofessor"])
            };
            if (withJoin)
                model.TeacherName = row["nome"].ToString();
            return model;
        }

        protected override void SetTable()
        {
            Table = "Materia";
        }

        public override List<SubjectViewModel> AdvancedQuery(int id)
        {
            SqlParameter[] p =
            {
                new SqlParameter("ID", id)
            };

            var table = HelperDAO.ExecuteProcSelect("spConsultaAvancadaMateria", p);
            var list = new List<SubjectViewModel>();
            foreach (DataRow dr in table.Rows)
            {
                list.Add(BuildModel(dr, true));
            }
            return list;
        }
    }
}