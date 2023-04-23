using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Repository.Interfaces;

public interface IAutorizaRepository
{
    Task<Usuarios> RegistrarUsuario(Usuarios usuario);
    Task<UsuarioToken> Login(LoginDTO loginDTO);
    Task RedefinirSenha(RedefinirSenhaDTO redefinirSenha);
}
