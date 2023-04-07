namespace WebApi.Models;

public class Tarefas
{
    public int ID_TAREFA { get; set; }
    public string? TITULO { get; set; }
    public string? DESCRICAO { get; set; }
    public DateTime DATA_CRIACAO { get; set; }
    public string? STATUS_TAREFAS { get; set; }
    public int ID_USUARIO { get; set; }
    public int ID_PROJETO { get; set; }
    public virtual Projetos PROJETO { get; set; }
    public virtual Usuarios USUARIO { get; set; }
}
