namespace WebApi.Models;

public class Projetos 
{
    public int IdProjeto {  get; set; }
    public string? NomeProjeto { get; set; }
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int IdUsuario { get; set; }
    public virtual Usuarios? Usuarios { get; set; }
}
