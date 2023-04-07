using System.Data.SqlClient;

namespace WebApi.Data;

public class AppDbContext
{
    public static SqlConnection OpenConection()
    {
        string cString = "Data Source=DESKTOP-10DDISU;Initial Catalog=GerenciadorTarefas;Integrated Security=True;Pooling=False;Encrypt=False;TrustServerCertificate=False;"
        SqlConnection connection = new SqlConnection(cString);
        connection.Open();
        return connection;
    }

    public static void CloseConnection(SqlConnection connection)
    {
        if(connection != null && connection.State != System.Data.ConnectionState.Closed)
        {
            connection.Close();
        }
    }
}
