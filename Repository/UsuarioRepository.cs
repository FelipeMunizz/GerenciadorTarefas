using System.Data.SqlClient;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Repository;

public class UsuarioRepository : IUsuarioRepository
{
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
}
