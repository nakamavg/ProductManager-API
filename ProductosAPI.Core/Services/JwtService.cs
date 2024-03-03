using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProductosAPI.Core.Models;

namespace ProductosAPI.Core.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private const int DefaultExpirationMinutes = 120;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _keyBytes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            var secretKey = _configuration["JWT_SECRET_KEY"] ?? throw new InvalidOperationException("JWT Secret key not configured");
            _issuer = _configuration["JWT_ISSUER_TOKEN"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            _audience = _configuration["JWT_AUDIENCE_TOKEN"] ?? throw new InvalidOperationException("JWT Audience not configured");
            _keyBytes = Encoding.UTF8.GetBytes(secretKey);
        }

        public string GenerateToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(_keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(GetExpirationMinutes());

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int GetExpirationMinutes()
        {
            if (int.TryParse(_configuration["JWT_EXPIRE_MINUTES"], out int minutes) && minutes > 0)
            {
                return minutes;
            }
            return DefaultExpirationMinutes;
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_keyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}