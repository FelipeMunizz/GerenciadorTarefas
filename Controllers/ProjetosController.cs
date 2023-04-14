using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjetosController : ControllerBase
{
    private readonly IConfiguration _config;

    public ProjetosController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("CriarProjeto")]
    public async Task<IActionResult> CriarProjeto([FromBody] Projetos projeto)
    {
        var query = @"
                         insert into PROJETOS (NOME_PROJETO, DESCRICAO, DATA_INICIO, ID_USUARIO)
                                       values (@NomeProjeto, @Descricao, @DataInicio, @IdUsuario)
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
                command.Parameters.AddWithValue("@DataInicio", DateTime.Now);
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

    [HttpPut("{id:int}", Name = "EditarCategoria")]
    public async Task<IActionResult> EditarProjeto([FromQuery]int id, [FromBody] Projetos projeto)
    {
        string query = @"
            update Projetos set NOME_PROJETO = @NomeProjeto, DESCRICAO = @Descricao, ID_USUARIO = @IdUsuario
        ";

        try
        {
            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@NomeProjeto", projeto.NomeProjeto);
                command.Parameters.AddWithValue("@Descricao", projeto.Descricao);

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
}
