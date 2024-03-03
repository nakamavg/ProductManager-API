using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Microsoft.IdentityModel.Tokens;
using ProductosAPI.Models;

namespace ProductosAPI.Filters
{
    public class JwtAuthorizationFilter : IAuthorizationFilter
    {
        private readonly Rol[] _rolesPermitidos;

        public JwtAuthorizationFilter(params Rol[] rolesPermitidos)
        {
            _rolesPermitidos = rolesPermitidos;
        }

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext, 
            CancellationToken cancellationToken, 
            Func<Task<HttpResponseMessage>> continuation)
        {
            // Verificar si hay token de autorización
            if (!actionContext.Request.Headers.Authorization?.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Token de autorización no proporcionado.")
                });
            }

            try
            {
                var token = actionContext.Request.Headers.Authorization.Parameter;
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
                var key = Encoding.UTF8.GetBytes(secretKey);

                // Validar token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"],
                    ValidateAudience = true,
                    ValidAudience = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                
                // Obtener rol del usuario
                var rolClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
                if (rolClaim == null)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Token no contiene información de rol.")
                    });
                }

                // Validar rol
                if (_rolesPermitidos.Length > 0 && !Enum.TryParse(rolClaim.Value, out Rol userRole))
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Rol inválido en el token.")
                    });
                }

                // Verificar si el usuario tiene el rol requerido
                if (_rolesPermitidos.Length > 0 && !_rolesPermitidos.Contains(userRole))
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {
                        Content = new StringContent("No tiene permiso para acceder a este recurso.")
                    });
                }

                // Establecer el principal de seguridad
                var identity = new ClaimsIdentity(jwtToken.Claims, "Bearer");
                Thread.CurrentPrincipal = new ClaimsPrincipal(identity);

                return continuation();
            }
            catch (SecurityTokenException ex)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent($"Token inválido: {ex.Message}")
                });
            }
            catch (Exception)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Error al validar el token.")
                });
            }
        }

        public bool AllowMultiple => true;
    }
}