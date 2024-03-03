using System.ComponentModel.DataAnnotations;
using ProductosAPI.Models;

namespace ProductosAPI.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RegisterDTO
    {
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public Rol Rol { get; set; }
    }

    public class TokenResponseDTO
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public Rol Rol { get; set; }
    }
}