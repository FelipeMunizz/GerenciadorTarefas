using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Produces("application/json")]
    public async Task<ActionResult<List<Projetos>>> ListarProjetos()
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        List<Projetos> projetos = await _repository.ListarPorUsuario(idUsuario);

        return Ok(projetos);
    }

    [HttpGet("ObterProjeto/{idProjeto:int}")]
    [Produces("application/json")]
    public async Task<ActionResult<Projetos>> ObterProjeto(int idProjeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        Projetos projeto = await _repository.ObterProjeto(idProjeto, idUsuario);

        return Ok(projeto);
    }

    [HttpPost("CriarProjeto")]
    [Produces("application/json")]
    public async Task<IActionResult> CriarProjeto([FromBody] Projetos projeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);

        await _repository.Add(projeto, idUsuario);

        return Ok(projeto);
    }

    [HttpPut("EditarProjeto")]
    [Produces("application/json")]
    public async Task<IActionResult> EditarProjeto([FromBody] Projetos projeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);

        await _repository.Update(projeto, idUsuario);

        return Ok(projeto);
    }

    [HttpDelete("FinalizarProjeto/{idProjeto:int}")]
    [Produces("application/json")]
    public async Task<IActionResult> FinalizarProjeto(int idProjeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        Projetos projeto = await _repository.FinalizarProjeto(idProjeto, idUsuario);

        return Ok(projeto);
    }

    [HttpDelete("DeletarProjeto/{idProjeto:int}")]
    [Produces("application/json")]
    public async Task<IActionResult> DeletarProjeto(int idProjeto)
    {
        int idUsuario = UsuariosHelpers.ObterIdUsuario(Request);
        await _repository.Delete(idProjeto, idUsuario);

        return Ok("Projeto deletado com sucesso");
    }
    [AllowAnonymous]
    [HttpGet("ListarUsuariosProjeto/{idProjeto:int}")]
    [Produces("application/json")]
    public async Task<object> ListarUsuariosProjeto(int idProjeto)
    {
        return await _repository.ListarUsuariosProjeto(idProjeto);
    }

    [HttpGet("ObterUsuarioProjetp/{idUsuarioProjeto:int}/{idProjeto:int}")]
    [Produces("application/json")]
    public async Task<ActionResult<UsuariosProjeto>> ObterUsuarioProjeto(int idUsuarioProjeto)
    {
        return await _repository.ObterUsuarioProjeto(idUsuarioProjeto);
    }

    [HttpPost("AdicionarUsuariosProjeto/{idProjeto:int}")]
    [Produces("application/json")]
    public async Task<IActionResult> AdicionarUsuariosProjeto(int idProjeto, [FromQuery] string usuario)
    {
        await _repository.AdicionarUsuarioProjeto(idProjeto, usuario, UsuariosHelpers.ObterIdUsuario(Request));
        return Ok("Usuario adicionado ao projeto com sucesso");
    }

    [HttpDelete("RemoverUsuarioProjeto/{idProjeto:int}/{idUsuarioProjeto:int}")]
    [Produces("application/json")]
    public async Task<IActionResult> RemoverUsuarioProjeto(int idProjeto, int idUsuarioProjeto)
    {
        await _repository.RemoverUsuarioProjeto(idProjeto, idUsuarioProjeto);
        return Ok();
    }
}
