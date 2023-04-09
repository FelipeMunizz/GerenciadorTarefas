namespace WebApi.Models;

public class Projetos 
{
    public int IdProjeto {  get; set; }
    public string? NomeProjeto { get; set; }
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
}
