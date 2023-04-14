using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebApi.Helpers;

public static class UsuariosHelpers
{
    public static int ObterIdUsuario(HttpRequest request)
    {

        #region Implementação antiga
        //var token = request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //var tokenHandler = new JwtSecurityTokenHandler();
        //var key = Encoding.ASCII.GetBytes(_config["Jwt:key"]);
        //var tokenValidationParameters = new TokenValidationParameters
        //{
        //    ValidateIssuerSigningKey = true,
        //    IssuerSigningKey = new SymmetricSecurityKey(key),
        //    ValidateIssuer = true,
        //    ValidIssuer = _config["TokenConfiguration:Issuer"],
        //    ValidateAudience = true,
        //    ValidAudience = _config["TokenConfiguration:Audience"],
        //    ValidateLifetime = true,
        //    ClockSkew = TimeSpan.Zero
        //};

        //SecurityToken validatedToken;
        //var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

        //var idUsuarioClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "idUsuario");
        //if (idUsuarioClaim == null)
        //{
        //    throw new Exception("Token inválido: idUsuario não encontrado");
        //}

        //if (!int.TryParse(idUsuarioClaim.Value, out int idUsuario))
        //{
        //    throw new Exception("Token inválido: idUsuario inválido");
        //}

        //return idUsuario;
        #endregion

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
