using WebApi.Models;

namespace WebApi.Repository.Interfaces;

public interface IProjetosRepository
{
    Task<List<Projetos>> ListarPorUsuario();
    Task<Projetos> ObterProjeto(int idProjeto);
    Task<Projetos> FinalizarProjeto(int idProjeto);
    Task Add(Projetos projeto, int idUsuario);
    Task Update(Projetos projeto);
    Task Delete(int idProjeto);
    Task<object> ListarUsuariosProjeto(int idProjeto);
    Task<UsuariosProjeto> ObterUsuarioProjeto(int idUsuarioProjeto);
    Task AdicionarUsuarioProjeto(int idProjeto, string user, int idUsuarioResponsavel = 0, bool responsavel = false);
    Task RemoverUsuarioProjeto(int idProjeto, int idUsuarioProjeto);
}
