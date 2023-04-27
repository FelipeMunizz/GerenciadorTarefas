using System.Data.SqlClient;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Helpers.Interfaces;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Repository;

public class UsuariosProjetoRepository : IUsuariosProjetoRepository
{
    private readonly IUsuarioRepository _usuario;
    private readonly IProjetosRepository _projetos;
    private readonly IEmailHelpers _email;

    public UsuariosProjetoRepository(IUsuarioRepository usuario, IEmailHelpers email, IProjetosRepository projetos)
    {
        _usuario = usuario;
        _email = email;
        _projetos = projetos;
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
        command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
        command.Parameters.AddWithValue("@Responsavel", resoponsavel);
        await connection.OpenAsync();

        var result = await command.ExecuteNonQueryAsync();

        if (result < 1)
            throw new Exception("Não foi possivel adicionar o usuario ao projeto");

        await connection.CloseAsync();

        if(resoponsavel == false)
        {
            Usuarios usuarioResponsavel = await _usuario.ObterUsuario(idUsuarioResponsavel);
            Projetos projeto = await _projetos.ObterProjeto(idProjeto, idUsuarioResponsavel);
            string assunto = "Você foi adicionado a um novo projeto";
            string mensagem = $"Olá {usuario.Nome}, você foi adicionado ao projeto {projeto.NomeProjeto}, por {usuarioResponsavel.Nome}.";
            bool sucesso = _email.Enviar(usuario.Email, assunto, mensagem);

            if (!sucesso)
                throw new Exception("Não foi possivel enviar o email");
        }
    }
}
