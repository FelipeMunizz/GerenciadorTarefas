using WebApi.Models;

namespace WebApi.Repository.Interfaces;

public interface IProjetosRepository
{
    Task<List<Projetos>> ListarPorUsuario(int idUsuario);
    Task<Projetos> ObterProjeto(int idProjeto, int idUsuario);
    Task<Projetos> FinalizarProjeto(int idProjeto, int idUsuario);
    Task AdicionarUsuarioProjeto(int idProjeto, string user, bool responsavel = false);
    Task Add(Projetos projeto, int idUsuario);
    Task Update(Projetos projeto, int idUsuario);
    Task Delete(int idProjeto, int idUsuario);
}
