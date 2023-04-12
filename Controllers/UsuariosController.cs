using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Helpers;
using WebApi.Helpers.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly IEmailHelpers _emailHelpers;

    public UsuariosController(IEmailHelpers emailHelpers)
    {
        _emailHelpers = emailHelpers;
    }

    [HttpGet("{id:int}", Name = "ObterUsuario")]
    public async Task<ActionResult<Usuarios>> Get(int id)
    {
        Usuarios usuario = new Usuarios();
        string query = "select * from USUARIOS where ID_USUARIO = @IdUsuario";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdUsuario", id);

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return NotFound("Usuario não encontrado.");
            }

            if (reader.Read())
            {
                usuario.IdUsuario = (int)reader["ID_USUARIO"];
                usuario.Nome = reader["NOME"].ToString();
                usuario.Sobrenome = reader["SOBRENOME"].ToString();
                usuario.Usuario = reader["USUARIO"].ToString();
                usuario.Email = reader["EMAIL"].ToString();
                usuario.DataCadastro = Convert.ToDateTime(reader["DATA_CADASTRO"]);
            }

            await connection.CloseAsync();
            return Ok(usuario);
        }
    }

    [HttpPost("AlterarSenha")]
    public async Task<IActionResult> AlterarSenha([FromBody]AlterarSenhaDTO alterarSenha)
    {
        if (alterarSenha.NovaSenha != alterarSenha.ConfirmarSenha)
            return NotFound("As senhas não conferem");

        bool validarSenha = SenhaHelpers.ValidaSenha(alterarSenha.NovaSenha);
        if (!validarSenha)
            return NotFound("As senhas não condiz com as regas");

        string novaSenha = SenhaHelpers.CriptografarSenha(alterarSenha.NovaSenha);

        Usuarios usuario = new Usuarios();
        string query = "select EMAIL from USUARIOS where USUARIO = @Usuario and SENHA = @Senha";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Usuario", alterarSenha.Usuario);
            command.Parameters.AddWithValue("@Senha", SenhaHelpers.CriptografarSenha(alterarSenha.SenhaAtual));

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return NotFound("Usuario não encontrado.");
            }

            if (reader.Read())
            {
                usuario.Email = reader["EMAIL"].ToString();
            }

            bool atualizarSenha = SenhaHelpers.AtualizarSenha(novaSenha, alterarSenha.Usuario, usuario.Email);
            if (!atualizarSenha)
                return NotFound("Não foi possivel atualizar a senha");

            string assunto = "Senha Alterada";
            string mensagem = $"Sua senha foi alterada {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}. Se você não alterou a senha entre em contato com o suporte.";
            bool emailEnviado = _emailHelpers.Enviar(usuario.Email, assunto, mensagem);

            if (!emailEnviado)
                return StatusCode(500, "Não foi possível enviar o email com a nova senha");

            await connection.CloseAsync();
            return Ok("Senha alterada com sucesso!");
        }
    }

    [HttpPut("AlterarUsuario")]
    public async Task<ActionResult<UsuarioDTO>> AlterarUsuario([FromBody]UsuarioDTO usuario)
    {
        string query = "select * from USUARIOS where ID_USUARIO = @IdUsuario";
        using(SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return NotFound("Usuario não encontrado");
            }                

            query = "update USUARIOS set NOME = @Nome, SOBRENOME = @Sobrenome, USUARIO = @Usuario, EMAIL = @Email where ID_USUARIO = @IdUsuario";

            command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Nome", usuario.Nome);
            command.Parameters.AddWithValue("@Sobrenome", usuario.Sobrenome);
            command.Parameters.AddWithValue("@Usuario", usuario.Usuario);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

            int result = await command.ExecuteNonQueryAsync();

            if(result < 0)
            {
                await connection.CloseAsync();
                return BadRequest("Não foi possivel atualizar as informações do usuario");
            }

            await connection.CloseAsync();
            return Ok(usuario);
        }
    }

    [HttpDelete("{id:int}", Name = "DeletarUsuario")]
    public async Task<IActionResult> DeletarUsuario(int id)
    {
        string query = "select * from USUARIOS where ID_USUARIO = @IdUsuario";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdUsuario", id);

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return NotFound("Usuário não encontrado.");
            }

            await reader.CloseAsync();

            query = "delete from USUARIOS where ID_USUARIO = @IdUsuario";

            command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdUsuario", id);

            int result = await command.ExecuteNonQueryAsync();

            await connection.CloseAsync();

            if (result < 0)
            {
                return BadRequest("Não foi possível remover o usuário.");
            }

            return Ok("Usuario excluido com sucesso!");
        }
    }
}
