using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebApi.Data;

namespace WebApi.Helpers;

public static class UsuariosHelpers
{
    public static int ObterIdUsuario(HttpRequest request)
    {
        StringValues authHeader;
        if (request.Headers.TryGetValue("Authorization", out authHeader) &&
            authHeader.FirstOrDefault()?.StartsWith("Bearer ") == true)
        {
            var token = authHeader.FirstOrDefault().Substring(7);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var idUsuarioClaim = jwt.Claims.FirstOrDefault(c => c.Type == "idUsuario");
            if (idUsuarioClaim != null && int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                return idUsuario;
            }
        }

        return 0;
    }

    public static bool UsuarioExistente(string user)
    {
        string query = "select ID_USUARIO from USUARIOS where USUARIO = @Usuario";
        using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Usuario", user);

            connection.Open();

            var reader = command.ExecuteReader();
            if (reader != null)
            {
                reader.Close();
                return true;
            }

            connection.Close();
            return false;
        }
    }
}
