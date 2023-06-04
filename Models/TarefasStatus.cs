using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class TarefasStatus
{
    public int TarefasStatusId { get; set; }
    public int StatusId { get; set; }
    public DateTime DataAlteracao { get; set; }
    public virtual Tarefas? Tarefas { get; set; }
    public virtual Status Status { get; set; }
}