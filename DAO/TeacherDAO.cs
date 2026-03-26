using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.DAO
{
    public class TeacherDAO : BaseDAO<TeacherViewModel>
    {
        protected override SqlParameter[] CreateParameters(TeacherViewModel model)
        {
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("id", model.ID);
            parameters[1] = new SqlParameter("nome", model.Name);
            parameters[2] = new SqlParameter("email", model.Email);
            parameters[3] = new SqlParameter("telefone", model.Phone);
            parameters[4] = new SqlParameter("cpf", model.CPF);
            return parameters;
        }

        protected override TeacherViewModel BuildModel(DataRow row, bool withJoin = false)
        {
            TeacherViewModel model = new TeacherViewModel()
            {
                ID = Convert.ToInt32(row["id"]),
                Name = row["nome"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["telefone"].ToString(),
                CPF = row["cpf"].ToString()
            };
            return model;
        }

        protected override void SetTable()
        {
            Table = "Professor";
        }

        public override int NextId()
        {
            var code = 0;
            var p = new SqlParameter[]
            {
                new SqlParameter("tabela", Table)
            };
            var table = HelperDAO.ExecuteProcSelect("spProximoId", p);

            if (table.Rows[0][0].ToString() != "1")
                code = Convert.ToInt32("2" + Convert.ToString(Convert.ToInt32(table.Rows[0][0].ToString().Substring(1)) + 1));
            else
                code = 21;
            return code;
        }

        //public List<TeacherViewModel> AdvancedQuery(int id)
        //{
        //    SqlParameter[] p =
        //    {
        //        new SqlParameter("ID", id)
        //    };
        //    var table = HelperDAO.ExecuteProcSelect("spConsultaAvancadaProfessor", p);
        //    var list = new List<TeacherViewModel>();
        //    foreach (DataRow dr in table.Rows)
        //    {
        //        list.Add(BuildModel(dr));
        //    }
        //    return list;
        //}
    }
}