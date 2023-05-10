using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Anexos
{
    [Key]
    public int IdAnexo { get; set; }
    public int IdTarefa { get; set; }
    public string? NomeAnexo { get; set; }
    public string? Dados { get; set; }
    public virtual Tarefas Tarefas { get; set; }
}
