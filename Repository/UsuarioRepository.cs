using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        Usuarios usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuariosId == idUsuario);

        if(usuario == null)
            throw new Exception("Usaurio não encontrado");

        return usuario;
    }

    public async Task<Usuarios> ObterUsuarioByUser(string user)
    {
        Usuarios usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Usuario == user);

        if (usuario == null)
            throw new Exception("Usaurio não encontrado");

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

    public async Task AlterarUsuario(Usuarios user)
    {
        Usuarios usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuariosId == user.UsuariosId);
        if (usuario == null) throw new Exception("Usuario não encontrado");

        usuario.Nome = user.Nome;
        usuario.Sobrenome = user.Sobrenome;
        usuario.Usuario = user.Usuario;
        usuario.Email = user.Email;

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task DeletarUsuario(int id)
    {
        Usuarios usuario = await ObterUsuario(id);
        if (usuario == null)
            throw new Exception("Usuario não encontrado");

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
    }

    private string GerarToken(Usuarios usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email),
            new Claim("idUsuario", usuario.UsuariosId.ToString()),
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
