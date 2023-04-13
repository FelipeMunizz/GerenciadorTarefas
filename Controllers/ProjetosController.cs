using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjetosController : ControllerBase
{
    [HttpPost("CriarProjeto")]
    public async Task<IActionResult> CriarProjeto([FromBody] Projetos projeto)
    {
        var query = @"
                         insert into PROJETOS (NOME_PROJETO, DESCRICAO, DATA_INICIO, DATA_FIM)
                                       values (@NomeProjeto, @Descricao, @DataInicio, @DataFim)
        ";
        try
        {
            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@NomeProjeto", projeto.NomeProjeto);
                command.Parameters.AddWithValue("@Descricao", projeto.Descricao);
                command.Parameters.AddWithValue("@DataInicio", DateTime.Now);

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
            update Projetos set NOME_PROJETO = @NomeProjeto, DESCRICAO = @Descricao
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
