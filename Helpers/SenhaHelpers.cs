using System.Security.Cryptography;
using System.Text;

namespace WebApi.Helpers;

public class SenhaHelpers
{
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
}
