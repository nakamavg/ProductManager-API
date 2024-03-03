using System;
using System.ComponentModel.DataAnnotations;

namespace ProductosAPI.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Accion { get; set; }
        
        [Required]
        public string Entidad { get; set; }
        
        [Required]
        public int EntidadId { get; set; }
        
        public string ValorAnterior { get; set; }
        
        public string ValorNuevo { get; set; }
        
        [Required]
        public int UsuarioId { get; set; }
        
        [Required]
        public DateTime FechaAccion { get; set; } = DateTime.Now;
        
        public string DireccionIP { get; set; }
    }
}