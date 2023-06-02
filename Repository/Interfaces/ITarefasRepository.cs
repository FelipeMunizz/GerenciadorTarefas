using WebApi.Models;

namespace WebApi.Repository.Interfaces;

public interface ITarefasRepository
{
    Task<List<Tarefas>> ListarTarefas(int idProjeto);
    Task<Tarefas> ObterTarefa(int idTarefa);
    Task Add(Tarefas tarefas);
    Task Update(Tarefas tarefas);
    Task Delete(int idTarefas);
}
