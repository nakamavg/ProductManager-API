using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProductosAPI.Models;

namespace ProductosAPI.Auth
{
    public class JwtAuthService
    {
        // Obtener la clave secreta del web.config
        private readonly string _secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
        private readonly string _issuer = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
        private readonly string _audience = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];

        public string GenerateJwtToken(Usuario usuario)
        {
            // Crear claims para el token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };

            // Crear llave para firmar el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            // Tiempo de expiración
            var expiration = DateTime.UtcNow.AddMinutes(GetExpiresInMinutes());

            // Crear el token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int GetExpiresInMinutes()
        {
            int expireMinutes = 120; // Default 2 hours
            if (int.TryParse(ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"], out int configMinutes))
            {
                expireMinutes = configMinutes;
            }
            return expireMinutes;
        }
    }
}