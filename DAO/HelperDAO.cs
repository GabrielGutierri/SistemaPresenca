using System.Data;
using System.Data.SqlClient;

namespace TrabalhoInterdisciplinar.DAO
{
    public static class HelperDAO
    {
        public static string StringCX;

        private static SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(StringCX);
            connection.Open();
            return connection;
        }

        public static DataTable ExecuteProcSelect(string procName, SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(procName, connection))
                {
                    if (parameters != null)
                        adapter.SelectCommand.Parameters.AddRange(parameters);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    connection.Close();
                    return table;
                }
            }
        }

        public static void ExecuteProc(string procName, SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand(procName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static DataTable ExecuteFunctionSelect(string query, SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    if (parameters != null)
                        adapter.SelectCommand.Parameters.AddRange(parameters);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    connection.Close();
                    return table;
                }
            }
        }
    }
}