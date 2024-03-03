using System.ComponentModel.DataAnnotations;
using ProductosAPI.Core.Models;

namespace ProductosAPI.Core.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class RegisterDTO
    {
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Login { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        public Rol Rol { get; set; }
    }

    public class TokenResponseDTO
    {
        public string Token { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public Rol Rol { get; set; }
    }
}