namespace WebApi.Repository.Interfaces;

public interface IUsuariosProjetoRepository
{
    Task AdicionarUsuarioProjeto(int idProjeto, string user, int idUsuarioResponsavel = 0, bool responsavel = false);
}
