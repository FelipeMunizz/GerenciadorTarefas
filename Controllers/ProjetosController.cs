using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
public class ProjetosController : ControllerBase
{
    private readonly IConfiguration _config;

    public ProjetosController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("ListarProjetos")]
    public async Task<object> ListarProjetos()
    {
        string query = @"
            select * from projetos where id_usuario = @IdUsuario
        ";
        List<Projetos> projetos = new List<Projetos>();

        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            await connection.OpenAsync();

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdUsuario", UsuariosHelpers.ObterIdUsuario(Request));

            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                Projetos projeto = new Projetos
                {
                    IdProjeto = (int)reader["ID_PROJETO"],
                    NomeProjeto = (string)reader["NOME_PROJETO"],
                    Descricao = (string)reader["DESCRICAO"],
                    DataInicio = (DateTime)reader["DATA_INICIO"],
                    IdUsuario = (int)reader["ID_USUARIO"]
                };
                projetos.Add(projeto);
            }
            reader.Close();
            await connection.CloseAsync();

            return Ok(projetos);
        }
    }

    [HttpGet("ObterProjeto/{idProjeto:int}")]
    public async Task<ActionResult<Projetos>> ObterProjeto(int idProjeto)
    {
        var query = @"
          select * from projetos where id_usuario = @IdUsuario and id_projeto = @IdProjeto
        ";

        Projetos projeto = new Projetos();

        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            await connection.OpenAsync();
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdUsuario", UsuariosHelpers.ObterIdUsuario(Request));
            command.Parameters.AddWithValue("@IdProjeto", idProjeto);
            var reader = await command.ExecuteReaderAsync();

            if (reader == null)
                return BadRequest("Projeto não foi localizado");

            if (reader.Read())
            {
                projeto.IdProjeto = (int)reader["ID_PROJETO"];
                projeto.NomeProjeto = (string)reader["NOME_PROJETO"];
                projeto.Descricao = (string)reader["DESCRICAO"];
                projeto.DataInicio = (DateTime)reader["DATA_INICIO"];
                projeto.DataFim = (DateTime)reader["DATA_FIM"];
                projeto.IdUsuario = (int)reader["ID_USUARIO"];
            }

            reader.Close();
            await connection.CloseAsync();

            return Ok(projeto);
        }
    }

    [HttpPost("CriarProjeto")]
    public async Task<IActionResult> CriarProjeto([FromBody] Projetos projeto)
    {
        var query = @"
                         insert into PROJETOS (NOME_PROJETO, DESCRICAO, DATA_INICIO, DATA_FIM, ID_USUARIO)
                                       values (@NomeProjeto, @Descricao, @DataInicio, @DataFim,  @IdUsuario)
        ";


        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);

        if (idUsuario == 0)
        {
            return Unauthorized("Você não esta autorizado a fazer requisições.");
        }

        try
        {
            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@NomeProjeto", projeto.NomeProjeto);
                command.Parameters.AddWithValue("@Descricao", projeto.Descricao);
                command.Parameters.AddWithValue("@DataInicio", DateTime.Now.ToString("yyyy/MM/dd"));
                command.Parameters.AddWithValue("@DataFim", DateTime.MinValue.ToString("yyyy/MM/dd"));
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                await connection.OpenAsync();
                int result = await command.ExecuteNonQueryAsync();

                if (result < 0)
                {
                    await connection.CloseAsync();
                    return BadRequest();
                }

                return Ok(projeto);
            }
        }
        catch (Exception e)
        {
            SqlConnection connection = new SqlConnection();
            connection.Close();

            return BadRequest($"Não foi possivel registrar o usuario: {e.Message}");
        }
    }

    [HttpPut("EditarProjeto")]
    public async Task<IActionResult> EditarProjeto([FromBody] Projetos projeto)
    {
        string query = @"
            update Projetos set NOME_PROJETO = @NomeProjeto, DESCRICAO = @Descricao where ID_USUARIO = @IdUsuario and ID_PROJETO = @IdProjeto
        ";

        try
        {
            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@NomeProjeto", projeto.NomeProjeto);
                command.Parameters.AddWithValue("@Descricao", projeto.Descricao);
                command.Parameters.AddWithValue("@IdUsuario", UsuariosHelpers.ObterIdUsuario(Request));

                await connection.OpenAsync();
                int result = await command.ExecuteNonQueryAsync();

                if (result < 0)
                {
                    await connection.CloseAsync();
                    return BadRequest();
                }

                return Ok(projeto);
            }
        }
        catch (Exception e)
        {
            return NotFound($"Não foi possivel editar o projeto. Erro: {e.Message}");
        }
    }

    [HttpDelete("FinalizarProjeto/{idProjeto:int}")]
    public async Task<IActionResult> FinalizarProjeto(int idProjeto)
    {
        string query = @"
            update Projetos set DATA_FIM = @DataFim where ID_USUARIO = @IdUsuario and ID_PROJETO = @IdProjeto
        ";

        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        Projetos projeto = ProjetoHelpers.ProjetoFinalizado(idProjeto, idUsuario);

        if (projeto == null)
            return BadRequest("Projeto não encontrado");

        if (projeto.DataFim > DateTime.MinValue)
            return BadRequest("Projeto já está finalizado");

        projeto.DataFim = DateTime.Now;

        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DataFim", projeto.DataFim.ToString("yyyy/MM/dd"));
            command.Parameters.AddWithValue("@IdProjeto", projeto.IdProjeto);
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            int result = await command.ExecuteNonQueryAsync();

            if (result < 0)
            {
                await connection.CloseAsync();
                return BadRequest();
            }
        }
        return Ok(projeto);
    }

    [HttpDelete("DeletarProjeto/{idProjeto:int}")]
    public async Task<IActionResult> DeletarProjeto(int idProjeto)
    {
        string query = @"
            delete from projetos where id_usuario = @IdUsuario and id_projeto = @IdProjeto
        ";

        using(SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString() ))
        {
            await connection.OpenAsync();

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdProjeto", idProjeto);
            command.Parameters.AddWithValue("@IdUsuario", UsuariosHelpers.ObterIdUsuario(Request));

            var result = await command.ExecuteNonQueryAsync();

            if (result <= 0)
                return BadRequest("Não foi possivel excluir o projeto");

            await connection.CloseAsync();
            return Ok("Projeto excluido com sucesso");
        }
    }
}
