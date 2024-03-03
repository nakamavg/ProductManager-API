using System.ComponentModel.DataAnnotations;

namespace ProductosAPI.Core.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = null!;
        
        [Required]
        [MaxLength(50)]
        public string Login { get; set; } = null!;
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;
        
        [Required]
        public string PasswordHash { get; set; } = null!;
        
        public Rol Rol { get; set; }
    }
}