namespace WebApi.Models;

public class HistoricoTarefas 
{
    public int IdHistoricoTarefa { get; set; }
    public int IdTarefa { get; set; }
    public DateTime DataAlteracao { get; set; }
    public string? DescricaoAlteracao { get; set; }
    public virtual Tarefas Tarefa { get; set; }
}
