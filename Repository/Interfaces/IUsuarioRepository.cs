using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuarios> RegistrarUsuario(Usuarios usuario);
    Task<Usuarios> RegistrarUsuarioGoogle(UsuarioGoogleDTO usuarioGoogle);
    Task<string> Login(LoginDTO loginDTO);
    Task RedefinirSenha(RedefinirSenhaDTO redefinirSenha);
    Task<Usuarios> ObterUsuario(int idUsuario);
    Task<Usuarios> ObterUsuarioByUser(string user);
    Task AlterarSenha(AlterarSenhaDTO alterarSenha);
    Task AlterarUsuario(Usuarios usuario);
    Task DeletarUsuario(int id);
}
