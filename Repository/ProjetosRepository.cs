using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using WebApi.Data;
using WebApi.Helpers;
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

        if (projeto == null)
            throw new Exception("Projeto não encontrado");

        if (projeto.DataFim > DateTime.MinValue)
            throw new Exception("Projeto já está finalizado");

        projeto.DataFim = DateTime.Now;

        _context.Projetos.Update(projeto); 
        await _context.SaveChangesAsync();

        return projeto;
    }

    public async Task Delete(int idProjeto)
    {
        Projetos projetos = await ObterProjeto(idProjeto);
        _context.Projetos.Remove(projetos);
        await _context.SaveChangesAsync();
    }

    public async Task<object> ListarUsuariosProjeto(int idProjeto)
    {
       return await _context.UsuariosProjetos.Where(x => x.ProjetoId == idProjeto).ToListAsync();
    }

    public async Task<UsuariosProjeto> ObterUsuarioProjeto(int idUsuarioProjeto)
    {
        return await _context.UsuariosProjetos.Where(x => x.UsuariosProjetoId == idUsuarioProjeto).FirstOrDefaultAsync();
    }

    public async Task AdicionarUsuarioProjeto(int idProjeto, string user, int idUsuarioResponsavel = 0, bool resoponsavel = false)
    {
        UsuariosProjeto usuarioProjeto = new UsuariosProjeto
        {
            ProjetoId = idProjeto,
            Responsavel = resoponsavel,
        };

        Usuarios usuario = await _usuario.ObterUsuarioByUser(user);

        usuarioProjeto.UsuariosId = usuario.UsuariosId;
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
        UsuariosProjeto usuariosProjeto = await ObterUsuarioProjeto(idUsuarioProjeto);
        _context.UsuariosProjetos.Remove(usuariosProjeto);
        await _context.SaveChangesAsync();
    }
}
