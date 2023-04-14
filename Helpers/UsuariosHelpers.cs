using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
}
