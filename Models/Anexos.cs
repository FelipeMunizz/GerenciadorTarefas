using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Anexos
{
    public int AnexosId { get; set; }
    public int TarefasId { get; set; }
    public string? NomeAnexo { get; set; }
    public string? Dados { get; set; }
    public virtual Tarefas Tarefas { get; set; }
}
