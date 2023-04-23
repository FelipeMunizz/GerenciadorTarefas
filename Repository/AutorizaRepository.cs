﻿using Microsoft.IdentityModel.Tokens;
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

public class AutorizaRepository : IAutorizaRepository
{
    private readonly IConfiguration _config;
    private readonly IEmailHelpers _emailHelpers;

    public AutorizaRepository(IConfiguration config, IEmailHelpers emailHelpers)
    {
        _config = config;
        _emailHelpers = emailHelpers;
    }

    public async Task<Usuarios> RegistrarUsuario(Usuarios usuario)
    {
        var senhaValidada = SenhaHelpers.ValidaSenha(usuario.Senha);
        if (!senhaValidada)
            throw new Exception("A senha deve conter (Deve ter mais de 8 caracteres, 1 caractere Maiusculo, 1 caractere numerico e 1 caractere especial)");

        if (UsuariosHelpers.UsuarioExistente(usuario.Usuario))
            throw new Exception($"Já existe um usuario: {usuario.Usuario}");

        try
        {
            string query = @"insert into USUARIOS (NOME, SOBRENOME, USUARIO, SENHA, EMAIL, DATA_CADASTRO) 
                                       values (@Nome, @Sobrenome, @Usuario, @Senha, @Email, @DataCadastro)";

            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Sobrenome", usuario.Sobrenome);
                command.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                command.Parameters.AddWithValue("@Senha", SenhaHelpers.CriptografarSenha(usuario.Senha));
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@DataCadastro", DateTime.Now);

                await connection.OpenAsync();
                int result = await command.ExecuteNonQueryAsync();

                if (result < 0)
                {
                    await connection.CloseAsync();
                    throw new Exception();
                }

                return usuario;
            }
        }
        catch (Exception e)
        {
            SqlConnection connection = new SqlConnection();
            connection.Close();

            throw new Exception($"Não foi possivel registrar o usuario: {e.Message}");
        }
    }

    public async Task<UsuarioToken> Login(LoginDTO loginDTO)
    {
        Usuarios usuario = new Usuarios();
        string query = "select * from USUARIOS where USUARIO = @Usuario and SENHA = @Senha";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Usuario", loginDTO.Usuario);
            command.Parameters.AddWithValue("@Senha", SenhaHelpers.CriptografarSenha(loginDTO.Senha));

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
        }

        return GerarToken(usuario);
    }

    public async Task RedefinirSenha(RedefinirSenhaDTO redefinirSenha)
    {
        string novaSenha = "";
        string query = "select ID_USUARIO from USUARIOS where EMAIL = @Email and USUARIO = @Usuario";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Email", redefinirSenha.Email);
            command.Parameters.AddWithValue("@Usuario", redefinirSenha.Usuario);

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                throw new Exception("Usuario não encontrado.");
            }

            novaSenha = SenhaHelpers.GenerateRandomPassword();
            var senhaCriptografada = SenhaHelpers.CriptografarSenha(novaSenha);

            bool senhaAtualizada = SenhaHelpers.AtualizarSenha(senhaCriptografada, redefinirSenha.Usuario, redefinirSenha.Email);

            if (!senhaAtualizada)
            {
                throw new Exception("Não foi possivel gerar uma nova senha");
            }

            string assunto = "Redefinição de Senha";
            string mensagem = $"Sua nova senha é: {novaSenha}";
            bool emailEnviado = _emailHelpers.Enviar(redefinirSenha.Email, assunto, mensagem);

            if (!emailEnviado)
                throw new Exception("Não foi possível enviar o email com a nova senha");
        }
    }

    private UsuarioToken GerarToken(Usuarios usuario)
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

        return new UsuarioToken()
        {
            Authenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            Message = "Token JWT Ok"
        };
    }
}
