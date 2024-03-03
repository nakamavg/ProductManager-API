using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductosAPI.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Nombre { get; set; }
        
        public decimal Alto { get; set; }
        
        public decimal Ancho { get; set; }
        
        public decimal Profundidad { get; set; }
        
        public decimal Volumen { get; set; }
        
        public decimal Peso { get; set; }
        
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        public bool Activo { get; set; } = true;
    }
}