using WebApi.Models;

namespace WebApi.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuarios> ObterUsuario(int idUsuario);
    Task<Usuarios> ObterUsuarioByUser(string user);
}
