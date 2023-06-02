using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
public class TarefasController : ControllerBase
{
    private readonly ITarefasRepository _tarefas;

    public TarefasController(ITarefasRepository tarefas)
    {
        _tarefas = tarefas;
    }

    [HttpGet("ListarTarefas/{idProjeto:int}")]
    [Produces("application/json")]
    public async Task<ActionResult<List<Tarefas>>> ListarTarefas(int idProjeto)
    {
        return await _tarefas.ListarTarefas(idProjeto);
    }

    [HttpGet("ObterTarefa/{idTarefa:int}")]
    [Produces("application/json")]
    public async Task<ActionResult<Tarefas>> ObterTarefa(int idTarefa)
    {
        return await _tarefas.ObterTarefa(idTarefa);
    }

    [HttpPost("CriarTarefa")]
    [Produces("application/json")]
    public async Task<IActionResult> CriarTarefa([FromBody] Tarefas tarefas)
    {
        await _tarefas.Add(tarefas);
        return Ok();
    }

    [HttpPut("EditarTarefa")]
    [Produces("application/json")]
    public async Task<ActionResult<Tarefas>> EditarTarefa([FromBody] Tarefas tarefas)
    {
        await _tarefas.Update(tarefas);
        return Ok(tarefas);
    }

    [HttpDelete("DeletarTarefa/{idTarefa:int}")]
    public async Task<ActionResult<bool>> DeletarTarefa(int idTarefa)
    {
        try
        {
            await _tarefas.Delete(idTarefa);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
