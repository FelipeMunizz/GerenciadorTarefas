using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Tarefas 
{
    public int TarefasId { get; set; }
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public string? StatusTarefa { get; set; }
    public int IdProjeto { get; set; }
    public virtual Projetos PROJETO { get; set; }
    public List<Anexos> Anexos { get; set; } = new List<Anexos>();
}
    