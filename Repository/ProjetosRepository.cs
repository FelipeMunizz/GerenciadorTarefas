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
        string query = @"
                update Projetos set DATA_FIM = @DataFim where ID_USUARIO = @IdUsuario and ID_PROJETO = @IdProjeto
            ";

        Projetos projeto = ProjetoHelpers.ProjetoFinalizado(idProjeto);

        if (projeto == null)
            throw new Exception("Projeto não encontrado");

        if (projeto.DataFim > DateTime.MinValue)
            throw new Exception("Projeto já está finalizado");

        projeto.DataFim = DateTime.Now;

        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

        return projeto;
    }

    public async Task Delete(int idProjeto)
    {
        string query = @"
            delete from projetos where id_usuario = @IdUsuario and id_projeto = @IdProjeto
            ";

        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            await connection.OpenAsync();

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdProjeto", idProjeto);

            var result = await command.ExecuteNonQueryAsync();

            if (result < 1)
                throw new Exception("Não foi possivel excluir o projeto");

            await connection.CloseAsync();
        }
    }

    public async Task<List<UsuariosProjeto>> ListarUsuariosProjeto(int idProjeto)
    {
        List<UsuariosProjeto> usuariosProjetos = new List<UsuariosProjeto>();

        string query = "select * from usuarios_projeto where id_projeto = @IdProjeto";

        SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString());
        SqlCommand command = new SqlCommand(query, connection);
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        command.Parameters.AddWithValue("@IdProjeto", idProjeto);

        var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            UsuariosProjeto us = new UsuariosProjeto
            {
                UsuariosProjetoId = (int)reader["ID_USUARIO_PROJETO"],
                UsuariosId = (int)reader["ID_USUARIO"],
                ProjetoId = (int)reader["ID_PROJETO"],
                Responsavel = (bool)reader["RESPONSAVEL"]
            };

            usuariosProjetos.Add(us);
        }
        return usuariosProjetos;
    }

    public async Task<UsuariosProjeto> ObterUsuarioProjeto(int idUsuarioProjeto)
    {
        UsuariosProjeto usuariosProjeto = new UsuariosProjeto();

        string query = "select * from usuarios_projeto where id_usuario_projeto = @IdUsuarioProjeto";

        SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString());
        SqlCommand command = new SqlCommand(query, connection);
        if (connection.State == ConnectionState.Closed)
            await connection.OpenAsync();

        command.Parameters.AddWithValue("@IdUsuarioProjeto", idUsuarioProjeto);

        var reader = await command.ExecuteReaderAsync();
        if (reader.Read())
        {
            usuariosProjeto.UsuariosProjetoId = (int)reader["ID_USUARIO_PROJETO"];
            usuariosProjeto.UsuariosId = (int)reader["ID_USUARIO"];
            usuariosProjeto.ProjetoId = (int)reader["ID_PROJETO"];
            usuariosProjeto.Responsavel = (bool)reader["RESPONSAVEL"];
        }

        return usuariosProjeto;
    }

    public async Task AdicionarUsuarioProjeto(int idProjeto, string user, int idUsuarioResponsavel = 0, bool resoponsavel = false)
    {
        string query = @"
                insert into USUARIOS_PROJETO (ID_PROJETO, ID_USUARIO, RESPONSAVEL)
                values (@IdProjeto, @IdUsuario, @Responsavel)
            "
        ;

        Usuarios usuario = await _usuario.ObterUsuarioByUser(user);

        if (usuario == null)
            throw new Exception("Usuario nao encontrado");

        SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString());
        SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@IdProjeto", idProjeto);
        command.Parameters.AddWithValue("@IdUsuario", usuario.UsuariosId);
        command.Parameters.AddWithValue("@Responsavel", resoponsavel);
        await connection.OpenAsync();

        var result = await command.ExecuteNonQueryAsync();

        if (result < 1)
            throw new Exception("Não foi possivel adicionar o usuario ao projeto");

        await connection.CloseAsync();

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
        string query = "delete from usuarios_projeto where id_usuario_projeto = @IdUsuarioProjeto and id_projeto = @IdProjeto";

        UsuariosProjeto usuariosProjeto = await ObterUsuarioProjeto(idUsuarioProjeto);
        if (usuariosProjeto.Responsavel)
            throw new Exception("O usuario responsavel não pode ser removido do projeto");

        SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString());
        SqlCommand command = new SqlCommand(query, connection);

        if (connection.State == ConnectionState.Closed)
            await connection.OpenAsync();

        command.Parameters.AddWithValue("@IdUsuarioProjeto", idUsuarioProjeto);
        command.Parameters.AddWithValue("@IdProjeto", idProjeto);

        int result = await command.ExecuteNonQueryAsync();
        if (result == 0) throw new Exception("Nçao foi possivel remover o usuario do projeto");
    }
}
