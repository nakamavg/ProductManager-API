using System.ComponentModel.DataAnnotations;

namespace ProductosAPI.Core.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;
        
        public decimal Alto { get; set; }
        
        public decimal Ancho { get; set; }
        
        public decimal Profundidad { get; set; }
        
        public decimal Volumen { get; set; }
        
        public decimal Peso { get; set; }
        
        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        public bool Activo { get; set; } = true;
    }
}