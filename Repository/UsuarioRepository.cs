using System.Data.SqlClient;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Helpers;
using WebApi.Helpers.Interfaces;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IEmailHelpers _emailHelpers;

    public UsuarioRepository(IEmailHelpers emailHelpers)
    {
        _emailHelpers = emailHelpers;
    }

    public async Task<Usuarios> ObterUsuario(int idUsuario)
    {
        Usuarios usuario = new Usuarios();
        string query = "select * from USUARIOS where ID_USUARIO = @IdUsuario";
        SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString());
        SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@IdUsuario", idUsuario);

        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            await connection.CloseAsync();
            throw new Exception("Usuario não encontrado.");
        }

        if (reader.Read())
        {
            usuario.IdUsuario = (int)reader["ID_USUARIO"];
            usuario.Nome = reader["NOME"].ToString();
            usuario.Sobrenome = reader["SOBRENOME"].ToString();
            usuario.Usuario = reader["USUARIO"].ToString();
            usuario.Email = reader["EMAIL"].ToString();
            usuario.DataCadastro = Convert.ToDateTime(reader["DATA_CADASTRO"]);
        }

        await connection.CloseAsync();
        return usuario;
    }

    public async Task<Usuarios> ObterUsuarioByUser(string user)
    {
        Usuarios usuario = new Usuarios();
        string query = "select * from USUARIOS where USUARIO = @Usuario";
        SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString());
        SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Usuario", user);

        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            await connection.CloseAsync();
            throw new Exception("Usuario não encontrado.");
        }

        if (reader.Read())
        {
            usuario.IdUsuario = (int)reader["ID_USUARIO"];
            usuario.Nome = reader["NOME"].ToString();
            usuario.Sobrenome = reader["SOBRENOME"].ToString();
            usuario.Usuario = reader["USUARIO"].ToString();
            usuario.Email = reader["EMAIL"].ToString();
            usuario.DataCadastro = Convert.ToDateTime(reader["DATA_CADASTRO"]);
        }

        await connection.CloseAsync();
        return usuario;
    }

    public async Task AlterarSenha(AlterarSenhaDTO alterarSenha)
    {
        if (alterarSenha.NovaSenha != alterarSenha.ConfirmarSenha)
            throw new Exception("A confirmação da senha deve ser igual a nova senha");

        bool validarSenha = SenhaHelpers.ValidaSenha(alterarSenha.NovaSenha);
        if (!validarSenha)
            throw new Exception("As senhas não condiz com as regas");

        string novaSenha = SenhaHelpers.CriptografarSenha(alterarSenha.NovaSenha);

        Usuarios usuario = new Usuarios();
        string query = "select EMAIL from USUARIOS where USUARIO = @Usuario and SENHA = @Senha";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Usuario", alterarSenha.Usuario);
            command.Parameters.AddWithValue("@Senha", SenhaHelpers.CriptografarSenha(alterarSenha.SenhaAtual));

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                throw new Exception("Usuario não encontrado.");
            }

            if (reader.Read())
            {
                usuario.Email = reader["EMAIL"].ToString();
            }

            bool atualizarSenha = SenhaHelpers.AtualizarSenha(novaSenha, alterarSenha.Usuario, usuario.Email);
            if (!atualizarSenha)
                throw new Exception("Não foi possivel atualizar a senha");

            string assunto = "Senha Alterada";
            string mensagem = $"Sua senha foi alterada {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}. Se você não alterou a senha entre em contato com o suporte.";
            bool emailEnviado = _emailHelpers.Enviar(usuario.Email, assunto, mensagem);

            if (!emailEnviado)
                throw new Exception("Não foi possível enviar o email com a nova senha");

            await connection.CloseAsync();
        }
    }

    public async Task AlterarUsuario(Usuarios usuario)
    {
        string query = "select * from USUARIOS where ID_USUARIO = @IdUsuario";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                await connection.CloseAsync();
               throw new Exception("Usuario não encontrado");
            }

            query = "update USUARIOS set NOME = @Nome, SOBRENOME = @Sobrenome, USUARIO = @Usuario, EMAIL = @Email where ID_USUARIO = @IdUsuario";

            command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Nome", usuario.Nome);
            command.Parameters.AddWithValue("@Sobrenome", usuario.Sobrenome);
            command.Parameters.AddWithValue("@Usuario", usuario.Usuario);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

            int result = await command.ExecuteNonQueryAsync();

            if (result < 0)
            {
                await connection.CloseAsync();
                throw new Exception("Não foi possivel atualizar as informações do usuario");
            }

            await connection.CloseAsync();            
        }
    }

    public async Task DeletarUsuario(int id)
    {
        Usuarios usuario = await ObterUsuario(id);
        if (usuario == null)
            throw new Exception("Usuario não encontrado");
        
        string query = "delete from USUARIOS where ID_USUARIO = @IdUsuario";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);            

            await connection.OpenAsync();            

            command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdUsuario", id);

            int result = await command.ExecuteNonQueryAsync();

            await connection.CloseAsync();

            if (result < 0)
            {
                throw new Exception("Não foi possível remover o usuário.");
            }            
        }
    }
}
