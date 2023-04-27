namespace WebApi.Repository.Interfaces;

public interface IUsuariosProjetoRepository
{
    Task AdicionarUsuarioProjeto(int idProjeto, string user, bool responsavel = false);
}
