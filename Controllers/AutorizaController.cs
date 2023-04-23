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
        string novaSenha = "";
        string query = "select ID_USUARIO from USUARIOS where EMAIL = @Email and USUARIO = @Usuario";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Email", redefinirSenha.Email);
            command.Parameters.AddWithValue("@Usuario", redefinirSenha.Usuario);

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return Unauthorized("Usuario não encontrado.");
            }

            novaSenha = SenhaHelpers.GenerateRandomPassword();
            var senhaCriptografada = SenhaHelpers.CriptografarSenha(novaSenha);

            bool senhaAtualizada = SenhaHelpers.AtualizarSenha(senhaCriptografada, redefinirSenha.Usuario, redefinirSenha.Email);

            if (!senhaAtualizada)
            {
                return NotFound("Não foi possivel gerar uma nova senha");
            }

            string assunto = "Redefinição de Senha";
            string mensagem = $"Sua nova senha é: {novaSenha}";
            bool emailEnviado = _emailHelpers.Enviar(redefinirSenha.Email, assunto, mensagem);

            if (!emailEnviado)
                return StatusCode(500, "Não foi possível enviar o email com a nova senha");
        }

        return Ok("Sua senha foi atualizada e enviada para o seu email. Tambem verifique sua caixa de spam");
    }
    
    
}
