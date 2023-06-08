using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApi.Data;
using WebApi.Helpers.Interfaces;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Repository;

public class ProjetosRepository : IProjetosRepository
{
    private readonly AppDbContext _context;
    private readonly IUsuarioRepository _usuario;
    private readonly IEmailHelpers _email;

    public ProjetosRepository(IUsuarioRepository usuario, IEmailHelpers email, AppDbContext context)
    {
        _usuario = usuario;
        _email = email;
        _context = context;
    }

    public async Task<List<Projetos>> ListarPorUsuario()
    {
        return await _context.Projetos.AsNoTracking().ToListAsync();
    }

    public async Task<Projetos> ObterProjeto(int idProjeto)
    {
        return await _context.Projetos.FirstOrDefaultAsync(x => x.ProjetosId == idProjeto);
    }

    public async Task Add(Projetos projeto, int idUsuario)
    {
        projeto.DataInicio = DateTime.Now;

        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync();

        Usuarios usuario = await _usuario.ObterUsuario(idUsuario);

        await AdicionarUsuarioProjeto(projeto.ProjetosId, usuario.Usuario, idUsuario, true);
    }

    public async Task Update(Projetos projeto)
    {
        _context.Projetos.Update(projeto);
        await _context.SaveChangesAsync();        
    }

    public async Task<Projetos> FinalizarProjeto(int idProjeto)
    {

        Projetos projeto = await ObterProjeto(idProjeto);

        projeto.DataFim = DateTime.Now;

        _context.Projetos.Update(projeto);
        await _context.SaveChangesAsync();

        return projeto;
    }

    public async Task Delete(int idProjeto)
    {
        Projetos projeto = await ObterProjeto(idProjeto);
        _context.Remove(projeto);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UsuariosProjeto>> ListarUsuariosProjeto(int idProjeto)
    {
        return await _context.UsuariosProjetos.Where(x => x.ProjetoId == idProjeto).AsNoTracking().ToListAsync();
    }

    public async Task<UsuariosProjeto> ObterUsuarioProjeto(int idUsuarioProjeto)
    {
        return await _context.UsuariosProjetos.FirstOrDefaultAsync(x => x.UsuariosProjetoId == idUsuarioProjeto);
    }

    public async Task AdicionarUsuarioProjeto(int idProjeto, string user, int idUsuarioResponsavel = 0, bool resoponsavel = false)
    {
        Usuarios usuario = await _usuario.ObterUsuarioByUser(user);

        if (usuario == null)
            throw new Exception("Usuario nao encontrado");

        UsuariosProjeto usuarioProjeto = new UsuariosProjeto
        {
            ProjetoId = idProjeto,
            UsuariosId = usuario.UsuariosId,
            Responsavel = resoponsavel
        };

        _context.UsuariosProjetos.Add(usuarioProjeto);
        await _context.SaveChangesAsync();

        if (!resoponsavel)
        {
            Usuarios usuarioResponsavel = await _usuario.ObterUsuario(idUsuarioResponsavel);
            Projetos projeto = await ObterProjeto(idProjeto);
            string assunto = "Você foi adicionado a um novo projeto";
            string mensagem = $"Olá {usuario.Nome}, você foi adicionado ao projeto {projeto.NomeProjeto}, por {usuarioResponsavel.NomeCompleto()}.";
            bool sucesso = _email.Enviar(usuario.Email, assunto, mensagem);

            if (!sucesso)
                throw new Exception("Não foi possivel enviar o email");
        }
    }

    public async Task RemoverUsuarioProjeto(int idProjeto, int idUsuarioProjeto)
    {
        UsuariosProjeto usuarioProjeto = await ObterUsuarioProjeto(idUsuarioProjeto);

        _context.UsuariosProjetos.Remove(usuarioProjeto);
        await _context.SaveChangesAsync();
    }
}
