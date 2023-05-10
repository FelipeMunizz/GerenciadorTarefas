using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class TarefasStatus
{
    [Key]
    public int IdTarefa { get; set; }
    public int IdStatus { get; set; }
    public DateTime DataAlteracao { get; set; }
    public virtual Tarefas Tarefas { get; set; }
    public virtual Status Status { get; set; }
}