using System;
using System.Data;
using System.Data.SqlClient;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.DAO
{
    public class LoginDAO : BaseDAO<LoginViewModel>
    {
        protected override SqlParameter[] CreateParameters(LoginViewModel model)
        {
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("id", model.ID);
            parameters[1] = new SqlParameter("senha", model.PasswordHash);
            return parameters;
        }

        protected override LoginViewModel BuildModel(DataRow row, bool withJoin = false)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ID = Convert.ToInt32(row["id"]),
                PasswordHash = row["senha"].ToString()
            };
            return model;
        }

        protected override void SetTable()
        {
            Table = "Login";
        }
    }
}