﻿using System.Data.SqlClient;
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

    public async Task AdicionarUsuarioProjeto(int idProjeto, string user, bool resoponsavel = false)
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
            Projetos projeto = await _projetos.ObterProjeto(idProjeto, )
            string assunto = "Você foi adicionado a um novo projeto";
            string mensagem = $"Ola {usuario.Nome}, você foi adicionado ao projeto {}";
            bool sucesso = _email.Enviar(usuario.Email, assunto, mensagem);
        }
    }
}
