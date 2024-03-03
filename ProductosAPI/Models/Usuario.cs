using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductosAPI.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Login { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        public Rol Rol { get; set; }
    }
}