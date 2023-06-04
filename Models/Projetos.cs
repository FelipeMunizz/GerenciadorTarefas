using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Projetos 
{
    public int ProjetosId {  get; set; }
    public string? NomeProjeto { get; set; }
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public List<UsuariosProjeto> UsuariosProjetos { get; set; } = new List<UsuariosProjeto>();
}
