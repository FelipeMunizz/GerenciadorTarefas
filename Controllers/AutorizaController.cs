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

namespace WebApi.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AutorizaController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IEmailHelpers _emailHelpers;

    public AutorizaController(IConfiguration config, IEmailHelpers emailHelpers)
    {
        _config = config;
        _emailHelpers = emailHelpers;
    }

    [HttpPost("Registrar")]
    public async Task<IActionResult> Registrar([FromBody] Usuarios usuario)
    {
        var senhaValidada = SenhaHelpers.ValidaSenha(usuario.Senha);
        if (!senhaValidada)
            return NotFound("A senha deve conter (Deve ter mais de 8 caracteres, 1 caractere Maiusculo, 1 caractere numerico e 1 caractere especial)");

        try
        {
            string query = @"insert into USUARIOS (NOME, SOBRENOME, USUARIO, SENHA, EMAIL, DATA_CADASTRO) 
                                       values (@Nome, @Sobrenome, @Usuario, @Senha, @Email, @DataCadastro)";

            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Sobrenome", usuario.Sobrenome);
                command.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                command.Parameters.AddWithValue("@Senha", SenhaHelpers.CriptografarSenha(usuario.Senha));
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@DataCadastro", DateTime.Now);

                await connection.OpenAsync();
                int result = await command.ExecuteNonQueryAsync();

                if (result < 0)
                {
                    await connection.CloseAsync();
                    return BadRequest();
                }

                return Ok(usuario);
            }
        }
        catch (Exception e)
        {
            SqlConnection connection = new SqlConnection();
            connection.Close();

            return BadRequest($"Não foi possivel registrar o usuario: {e.Message}");
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        string query = "select * from USUARIOS where EMAIL = @Email and SENHA = @Senha";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Email", loginDTO.Email);
            command.Parameters.AddWithValue("@Senha", SenhaHelpers.CriptografarSenha(loginDTO.Senha));

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if(!reader.HasRows)
            {
                await connection.CloseAsync();
                return Unauthorized("Usuario não encontrado.");
            }
        }

        return Ok(GerarToken(loginDTO));
    }

    [HttpPost("RedefinirSenha")]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDTO redefinirSenha)
    {
        string novaSenha = "";
        string query = "select * from USUARIOS where EMAIL = @Email and USUARIO = @Usuario";
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
    
    private UsuarioToken GerarToken(LoginDTO loginDTO)
    {
        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, loginDTO.Email),
                new Claim("meuPet", "bili"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:key"]));

        var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiracao = _config["TokenConfiguration:ExpireHours"];
        var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao));

        JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["TokenConfiguration:Issuer"],
                audience: _config["TokenConfiguration:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credenciais);

        return new UsuarioToken()
        {
            Authenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            Message = "Token JWT Ok"
        };
    }
}
