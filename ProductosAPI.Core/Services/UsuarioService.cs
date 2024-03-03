using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProductosAPI.Core.Data;
using ProductosAPI.Core.DTOs;
using ProductosAPI.Core.Models;

namespace ProductosAPI.Core.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public UsuarioService(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<Usuario> RegistrarUsuarioAsync(RegisterDTO registerDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new InvalidOperationException("Ya existe un usuario con este correo electrónico.");
            }

            var usuario = new Usuario
            {
                Nombre = registerDto.Nombre,
                Login = registerDto.Login,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                Rol = registerDto.Rol
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<TokenResponseDTO?> AutenticarUsuarioAsync(LoginDTO loginDto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (usuario == null || !VerifyPassword(loginDto.Password, usuario.PasswordHash))
            {
                return null;
            }

            var token = _jwtService.GenerateToken(usuario);
            
            return new TokenResponseDTO
            {
                Token = token,
                ExpiresIn = _jwtService.GetExpirationMinutes() * 60,
                UserId = usuario.Id,
                UserName = usuario.Nombre,
                UserEmail = usuario.Email,
                Rol = usuario.Rol
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == storedHash;
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
    }
}