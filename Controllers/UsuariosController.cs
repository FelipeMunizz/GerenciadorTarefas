using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repository;

    public UsuariosController(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    [AllowAnonymous]
    [HttpPost("Registrar")]
    public async Task<IActionResult> Registrar([FromBody] Usuarios usuario)
    {
        Usuarios user = await _repository.RegistrarUsuario(usuario);
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("RegistrarGoogle")]
    public async Task<IActionResult> RegistrarGoogle([FromBody] UsuarioGoogleDTO usuarioGoogle)
    {
        Usuarios usuario = await _repository.RegistrarUsuarioGoogle(usuarioGoogle);
        return Ok(usuario);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<string> Login([FromBody] LoginDTO loginDTO)
    {
        return await _repository.Login(loginDTO); 
    }

    [AllowAnonymous]
    [HttpPut("RedefinirSenha")]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDTO redefinirSenha)
    {
        await _repository.RedefinirSenha(redefinirSenha);

        return Ok("Sua senha foi atualizada e enviada para o seu email. Tambem verifique sua caixa de spam");
    }

    [AllowAnonymous]
    [HttpGet("EnviarUsuario")]
    public async Task<IActionResult> EnviarUsuario(string email)
    {
        await _repository.EnviarUsuario(email);
        return Ok();
    }

    [HttpGet("ObterUsuario/{id:int}")]
    public async Task<ActionResult<Usuarios>> ObterUsuario(int id)
    {
        Usuarios usuario = await _repository.ObterUsuario(id);

        return Ok(usuario);
    }

    [HttpPut("AlterarSenha")]
    public async Task<IActionResult> AlterarSenha([FromBody]AlterarSenhaDTO alterarSenha)
    {
        await _repository.AlterarSenha(alterarSenha);
        return Ok("Senha alterada com sucesso");
    }

    [HttpPut("AlterarUsuario")]
    public async Task<IActionResult> AlterarUsuario([FromBody]Usuarios usuario)
    {
        await _repository.AlterarUsuario(usuario);
        return Ok("Usuario alterado com sucesso");
    }

    [HttpDelete("DeletarUsuario/{id:int}")]
    public async Task<IActionResult> DeletarUsuario(int id)
    {
        await _repository.DeletarUsuario(id);
        return Ok("Usuario deletado com sucesso");
    }
}
