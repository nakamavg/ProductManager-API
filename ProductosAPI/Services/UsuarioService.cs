using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ProductosAPI.Data;
using ProductosAPI.DTOs;
using ProductosAPI.Models;

namespace ProductosAPI.Services
{
    public class UsuarioService
    {
        private readonly ProductosDbContext _context;

        public UsuarioService()
        {
            _context = new ProductosDbContext();
        }

        public async Task<Usuario> RegistrarUsuarioAsync(RegisterDTO registerDTO)
        {
            // Verificar si ya existe un usuario con el mismo email
            if (await _context.Usuarios.AnyAsync(u => u.Email == registerDTO.Email))
            {
                throw new InvalidOperationException("Ya existe un usuario con este correo electrónico.");
            }

            // Crear el hash de la contraseña
            string passwordHash = HashPassword(registerDTO.Password);

            // Crear nuevo usuario
            var usuario = new Usuario
            {
                Nombre = registerDTO.Nombre,
                Login = registerDTO.Login,
                Email = registerDTO.Email,
                PasswordHash = passwordHash,
                Rol = registerDTO.Rol
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario> AutenticarUsuarioAsync(string email, string password)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null || !VerifyPassword(password, usuario.PasswordHash))
            {
                return null; // Autenticación fallida
            }

            return usuario; // Autenticación exitosa
        }

        // Hash de contraseña usando SHA-256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Verificar hash de contraseña
        private bool VerifyPassword(string password, string storedHash)
        {
            string passwordHash = HashPassword(password);
            return passwordHash == storedHash;
        }

        public async Task<Usuario> ObtenerUsuarioPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
    }
}