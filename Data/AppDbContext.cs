using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public AppDbContext()
    {
        
    }

    #region DbSet's
    public DbSet<Anexos> Anexos { get; set; }
    public DbSet<Projetos> Projetos { get; set; }
    public DbSet<Status> Status { get; set; }
    public DbSet<Tarefas> Tarefas { get; set; }
    public DbSet<TarefasStatus> TarefasStatus { get; set; }
    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<UsuariosProjeto> UsuariosProjetos { get; set; }
    #endregion

    public static string GetConnectionString()
    {
        string not = "Data Source=DESKTOP-V672319\\SQLEXPRESS;Initial Catalog=GerenciadorTarefas;Integrated Security=True;Pooling=False;Encrypt=False;TrustServerCertificate=False;";
        string pc = "Data Source=DESKTOP-10DDISU;Initial Catalog=GerenciadorTarefas;Integrated Security=True;Pooling=False;Encrypt=False;TrustServerCertificate=False;";
        string produce = "workstation id=taskmaster.mssql.somee.com;packet size=4096;user id=TaskMaster_SQLLogin_1;pwd=fjte8c7tuh;data source=taskmaster.mssql.somee.com;persist security info=False;initial catalog=taskmaster;Pooling=False;Encrypt=False;TrustServerCertificate=False;";
        return produce;
    }
}
