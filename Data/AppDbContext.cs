using System.Data.SqlClient;

namespace WebApi.Data;

public class AppDbContext
{
    public static string GetConnectionString()
    {
        return "Data Source=DESKTOP-10DDISU;Initial Catalog=GerenciadorTarefas;Integrated Security=True;Pooling=False;Encrypt=False;TrustServerCertificate=False;";
    }
}
