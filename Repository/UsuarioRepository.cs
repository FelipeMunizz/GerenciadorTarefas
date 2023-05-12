using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Helpers;
using WebApi.Helpers.Interfaces;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IConfiguration _config;
    private readonly IEmailHelpers _emailHelpers;
    private readonly AppDbContext _context;

    public UsuarioRepository(IConfiguration config, IEmailHelpers emailHelpers, AppDbContext context)
    {
        _config = config;
        _emailHelpers = emailHelpers;
        _context = context;
    }

    public async Task RegistrarUsuario(Usuarios usuario)
    {
        var senhaValidada = SenhaHelpers.ValidaSenha(usuario.Senha);
        if (!senhaValidada)
            throw new Exception("A senha deve conter (Deve ter mais de 8 caracteres, 1 caractere Maiusculo, 1 caractere numerico e 1 caractere especial)");

        Usuarios userExiste = _context.Usuarios.FirstOrDefault(u => u.Usuario == usuario.Usuario);
        if (userExiste != null)
            throw new Exception("Usuario já existe");

        usuario.Senha = SenhaHelpers.CriptografarSenha(usuario.Senha);
        usuario.DataCadastro = DateTime.Now;

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<Usuarios> RegistrarUsuarioGoogle(UsuarioGoogleDTO usuarioGoogle)
    {
        Usuarios usuario = await ObterUsuarioByUser(usuarioGoogle.Usuario);
        if (usuario != null)
            throw new Exception("Usuario já existente.");
        usuario = new Usuarios
        {
            Nome = usuarioGoogle.Nome,
            Sobrenome = usuarioGoogle.Sobrenome,
            Usuario = usuarioGoogle.Usuario,
            Email = usuarioGoogle.Email,
            Senha = SenhaHelpers.GenerateRandomPassword()
        };

        await RegistrarUsuario(usuario);

        return usuario;
    }

    public async Task<string> Login(LoginDTO loginDTO)
    {
        Usuarios usuario = _context.Usuarios.FirstOrDefault(x => x.Usuario == loginDTO.Usuario &&
                                                                 x.Senha == SenhaHelpers.CriptografarSenha(loginDTO.Senha));

        if (usuario == null)
            throw new Exception("Usuario não encontrado.");

        return GerarToken(usuario);
    }

    public async Task RedefinirSenha(RedefinirSenhaDTO redefinirSenha)
    {
        Usuarios usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Usuario == redefinirSenha.Usuario && x.Email == redefinirSenha.Email);
        if (usuario == null)
            throw new Exception("Usuario não encontrado");

        string novaSenha = SenhaHelpers.GenerateRandomPassword();

        string senhaCriptografada = SenhaHelpers.CriptografarSenha(novaSenha);

        usuario.Senha = senhaCriptografada;
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        string assunto = "Redefinição de Senha";
        string mensagem = $"Sua nova senha é: {novaSenha}";
        bool emailEnviado = _emailHelpers.Enviar(redefinirSenha.Email, assunto, mensagem);

        if (!emailEnviado)
            throw new Exception("Não foi possível enviar o email com a nova senha");
    }

    public async Task EnviarUsuario(string email)
    {
        Usuarios usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
        if(usuario == null)
            throw new Exception("Usaurio não infomrado (Contate o suporte)");

        if (!string.IsNullOrEmpty(usuario.Usuario))
        {
            string assunto = "Esqueceu seu usuario no Task Master?";
            string msg = $"Seu usaurio é {usuario.Usuario}";
            bool sucesso = _emailHelpers.Enviar(email, assunto, msg);
            if (!sucesso)
                throw new Exception("Não foi possivel enviar o usuario por email");
        }
    }

    public async Task<Usuarios> ObterUsuario(int idUsuario)
    {
        Usuarios usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

        if(usuario == null)
            throw new Exception("Usaurio não encontrado");

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

        Usuarios usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Usuario == alterarSenha.Usuario &&
                                                                            x.Senha == SenhaHelpers.CriptografarSenha(alterarSenha.SenhaAtual));
        if (usuario == null) throw new Exception("Usuario não encontrado");

        usuario.Senha = SenhaHelpers.CriptografarSenha(alterarSenha.NovaSenha);

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        string assunto = "Senha Alterada";
        string mensagem = $"Sua senha foi alterada {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}. Se você não alterou a senha entre em contato com o suporte.";
        bool emailEnviado = _emailHelpers.Enviar(usuario.Email, assunto, mensagem);

        if (!emailEnviado)
            throw new Exception("Não foi possível enviar o email com a nova senha");

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

    private string GerarToken(Usuarios usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email),
            new Claim("idUsuario", usuario.IdUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:key"]));

        var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiracao = _config["TokenConfiguration:ExpireHours"];
        var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao));

        JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["TokenConfiguration:Issuer"],
                audience: _config["TokenConfiguration:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
