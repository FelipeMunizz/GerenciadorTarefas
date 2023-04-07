namespace WebApi.Models;

public class Projetos
{
    public int ID_PROJETO {  get; set; }
    public string? NOME_PROJETO { get; set; }
    public string? DESCRICAO { get; set; }
    public DateTime DATA_INICIO { get; set; }
    public DateTime DATA_FIM { get; set; }
}
