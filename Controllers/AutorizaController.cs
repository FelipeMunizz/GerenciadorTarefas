using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Helpers;
using WebApi.Helpers.Interfaces;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AutorizaController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IEmailHelpers _emailHelpers;
    private readonly IAutorizaRepository _autoriza;

    public AutorizaController(IConfiguration config, IEmailHelpers emailHelpers, IAutorizaRepository autoriza)
    {
        _config = config;
        _emailHelpers = emailHelpers;
        _autoriza = autoriza;
    }

    [HttpPost("Registrar")]
    public async Task<IActionResult> Registrar([FromBody] Usuarios usuario)
    {
        Usuarios user = await _autoriza.RegistrarUsuario(usuario);
        return Ok(user);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        UsuarioToken login = await _autoriza.Login(loginDTO);
        return Ok(login);
    }

    [HttpPut("RedefinirSenha")]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDTO redefinirSenha)
    {
        await _autoriza.RedefinirSenha(redefinirSenha);

        return Ok("Sua senha foi atualizada e enviada para o seu email. Tambem verifique sua caixa de spam");
    }
    
    
}
