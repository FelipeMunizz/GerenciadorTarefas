using WebApi.Models;

namespace WebApi.Repository.Interfaces;

public interface IProjetosRepository
{
    Task<List<Projetos>> ListarPorUsuario(int idUsuario);
    Task<Projetos> ObterProjeto(int idProjeto, int idUsuario);
    Task<Projetos> FinalizarProjeto(int idProjeto, int idUsuario);
    Task Add(Projetos projeto, int idUsuario);
    Task Update(Projetos projeto, int idUsuario);
    Task Delete(int idProjeto, int idUsuario);
    Task AdicionarUsuarioProjeto(int idProjeto, string user, int idUsuarioResponsavel = 0, bool responsavel = false);
    Task RemoverUsuarioProjeto(int idProjeto, int idUsuarioProjeto, int idResponsavel);
}
