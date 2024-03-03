using System.Security.Cryptography;
using System.Text;
using ProductosAPI.Core.Models;

namespace ProductosAPI.Core.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Si ya hay usuarios, la base de datos ya ha sido inicializada
            if (context.Usuarios.Any())
            {
                return;
            }

            // Crear usuarios iniciales
            var usuarios = new[]
            {
                new Usuario
                {
                    Nombre = "Administrador",
                    Login = "admin",
                    Email = "admin@test.com",
                    PasswordHash = HashPassword("admin123"),
                    Rol = Rol.Admin
                },
                new Usuario
                {
                    Nombre = "Usuario Regular",
                    Login = "usuario",
                    Email = "usuario@test.com",
                    PasswordHash = HashPassword("usuario123"),
                    Rol = Rol.Usuario
                }
            };

            context.Usuarios.AddRange(usuarios);

            // Crear productos de ejemplo
            var productos = new[]
            {
                new Producto
                {
                    Nombre = "Producto 1",
                    Alto = 10,
                    Ancho = 20,
                    Profundidad = 30,
                    Volumen = 6000,
                    Peso = 2.5m,
                    FechaCreacion = DateTime.Now.AddDays(-30),
                    Activo = true
                },
                new Producto
                {
                    Nombre = "Producto 2",
                    Alto = 5,
                    Ancho = 15,
                    Profundidad = 25,
                    Volumen = 1875,
                    Peso = 1.2m,
                    FechaCreacion = DateTime.Now.AddDays(-15),
                    Activo = true
                }
            };

            context.Productos.AddRange(productos);
            context.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}