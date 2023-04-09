﻿using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using WebApi.Data;

namespace WebApi.Helpers;

public class SenhaHelpers
{
    private static readonly Random random = new Random();

    private static readonly string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly string lowercase = "abcdefghijklmnopqrstuvwxyz";
    private static readonly string digits = "0123456789";
    private static readonly string specials = "!@#$%^&*()_+-=[]{}|;':\"<>,.?/~`";

    public static string GenerateRandomPassword(int length = 8)
    {
        StringBuilder passwordBuilder = new StringBuilder();

        passwordBuilder.Append(GetRandomChar(uppercase));
        passwordBuilder.Append(GetRandomChar(digits));
        passwordBuilder.Append(GetRandomChar(specials));

        for (int i = 3; i < length; i++)
        {
            string charSet = uppercase + lowercase + digits + specials;
            passwordBuilder.Append(GetRandomChar(charSet));
        }

        return new string(passwordBuilder.ToString().OrderBy(c => random.Next()).ToArray());
    }

    private static char GetRandomChar(string charSet)
    {
        int index = random.Next(charSet.Length);
        return charSet[index];
    }

    public static bool ValidaSenha(string senha)
    {
        if(senha.Length < 8) return false;

        if(!senha.Any(char.IsUpper)) return false;

        if (!senha.Any(char.IsDigit)) return false;

        if (!senha.Any(c => !char.IsLetterOrDigit(c))) return false;

        return true;
    }

    public static string CriptografarSenha(string senha)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(senha));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static bool AtualizarSenha(string senha, string usuario, string email)
    {
        var query = "update usuarios set SENHA = @Senha where USUARIO = @Usuario and EMAIL = @Email";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Usuario", usuario);
            command.Parameters.AddWithValue("@Senha", senha);

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            
            int result = command.ExecuteNonQuery();

            if (result < 0)
            {
                connection.Close();
                return false;
            }

            return true;
        }
    }
}
