using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
public class ProjetosController : ControllerBase
{

    private readonly IProjetosRepository _repository;

    public ProjetosController(IProjetosRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("ListarProjetos")]
    public async Task<ActionResult<List<Projetos>>> ListarProjetos()
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        List<Projetos> projetos = await _repository.ListarPorUsuario(idUsuario);

        return Ok(projetos);
    }

    [HttpGet("ObterProjeto/{idProjeto:int}")]
    public async Task<ActionResult<Projetos>> ObterProjeto(int idProjeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        Projetos projeto = await _repository.ObterProjeto(idProjeto, idUsuario);

        return Ok(projeto);
    }

    [HttpPost("CriarProjeto")]
    public async Task<IActionResult> CriarProjeto([FromBody] Projetos projeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);

        await _repository.Add(projeto, idUsuario);

        return Ok(projeto);
    }

    [HttpPut("EditarProjeto")]
    public async Task<IActionResult> EditarProjeto([FromBody] Projetos projeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);

        await _repository.Update(projeto, idUsuario);

        return Ok(projeto);
    }

    [HttpDelete("FinalizarProjeto/{idProjeto:int}")]
    public async Task<IActionResult> FinalizarProjeto(int idProjeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        Projetos projeto = await _repository.FinalizarProjeto(idProjeto, idUsuario);

        return Ok(projeto);
    }

    [HttpDelete("DeletarProjeto/{idProjeto:int}")]
    public async Task<IActionResult> DeletarProjeto(int idProjeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        await _repository.Delete(idProjeto, idUsuario);

        return Ok("Projeto deletado com sucesso");
    }
}
