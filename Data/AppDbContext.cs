using System.Data.SqlClient;

namespace WebApi.Data;

public class AppDbContext
{
    public static string GetConnectionString()
    {
        string not = "Data Source=DESKTOP-V672319\\SQLEXPRESS;Initial Catalog=GerenciadorTarefas;Integrated Security=True;Pooling=False;Encrypt=False;TrustServerCertificate=False;";
        string pc = "Data Source=DESKTOP-10DDISU;Initial Catalog=GerenciadorTarefas;Integrated Security=True;Pooling=False;Encrypt=False;TrustServerCertificate=False;";
        return not;
    }
}
