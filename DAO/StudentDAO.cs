using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.DAO
{
    public class StudentDAO : BaseDAO<StudentViewModel>
    {
        protected override SqlParameter[] CreateParameters(StudentViewModel model)
        {
            object imageBytes = model.ImageBytes;
            if (imageBytes == null)
                imageBytes = DBNull.Value;

            SqlParameter[] parameters = new SqlParameter[7];
            parameters[0] = new SqlParameter("id", model.ID);
            parameters[1] = new SqlParameter("nome", model.Name);
            parameters[2] = new SqlParameter("email", model.Email);
            parameters[3] = new SqlParameter("telefone", model.Phone);
            parameters[4] = new SqlParameter("cpf", model.Cpf);
            parameters[5] = new SqlParameter("imagem", imageBytes);
            parameters[6] = new SqlParameter("IdBiometria", model.BiometricId);

            return parameters;
        }

        protected override StudentViewModel BuildModel(DataRow row, bool withJoin = false)
        {
            StudentViewModel model = new StudentViewModel()
            {
                ID = Convert.ToInt32(row["id"]),
                Name = row["nome"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["telefone"].ToString(),
                Cpf = row["cpf"].ToString()
            };
            if (row["idBiometria"] != DBNull.Value)
                model.BiometricId = Convert.ToInt32(row["IdBiometria"]);
            if (row["imagem"] != DBNull.Value)
                model.ImageBytes = row["imagem"] as byte[];
            return model;
        }

        protected override void SetTable()
        {
            Table = "Aluno";
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
                code = Convert.ToInt32("3" + Convert.ToString(Convert.ToInt32(table.Rows[0][0].ToString().Substring(1)) + 1));
            else
                code = 31;
            return code;
        }

        public int NextBiometricId()
        {
            var table = HelperDAO.ExecuteProcSelect("spProximoIdBiometria", null);
            return Convert.ToInt32(table.Rows[0][0].ToString());
        }

        //public List<StudentViewModel> AdvancedQuery(int id)
        //{
        //    SqlParameter[] p =
        //    {
        //        new SqlParameter("ID", id)
        //    };
        //    var table = HelperDAO.ExecuteProcSelect("spConsultaAvancadaAluno", p);
        //    var list = new List<StudentViewModel>();
        //    foreach (DataRow dr in table.Rows)
        //    {
        //        list.Add(BuildModel(dr));
        //    }
        //    return list;
        //}
    }
}