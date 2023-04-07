using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    [Table("HISTORIOC_TAREFAS")]
    public class HistoricoTarefas
    {
        public int ID_HISTORICO_TAREFA { get; set; }
        public int ID_TAREFA { get; set; }
        public DateTime DATA_ALTERACAO { get; set; }
        public string? DESCRICAO_ALTERACAO { get; set; }
        public virtual Tarefas TAREFA { get; set; }
    }
}
