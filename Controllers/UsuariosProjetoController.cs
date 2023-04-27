using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Repository.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuariosProjetoController : ControllerBase
{
    private readonly IUsuariosProjetoRepository _repository;

    public UsuariosProjetoController(IUsuariosProjetoRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("AdicionarUsuariosProjeto/{idProjeto:int}")]
    [Produces("application/json")]
    public async Task<IActionResult> AdicionarUsuariosProjeto(int idProjeto, [FromQuery] string usuario)
    {
        await _repository.AdicionarUsuarioProjeto(idProjeto, usuario, UsuariosHelpers.ObterIdUsuario(Request));
        return Ok("Usuario adicionado ao projeto com sucesso");
    }
}
