using System;
using System.ComponentModel.DataAnnotations;

namespace ProductosAPI.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Alto { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Ancho { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Profundidad { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Volumen { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Peso { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        
        public bool Activo { get; set; }
    }

    public class ProductoCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Alto { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Ancho { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Profundidad { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Peso { get; set; }
    }

    public class ProductoUpdateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Alto { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Ancho { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Profundidad { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Peso { get; set; }
    }

    public class PaginacionDTO
    {
        private int _pagina = 1;
        private int _registrosPorPagina = 10;
        private readonly int _maximoRegistrosPorPagina = 50;

        public int Pagina
        {
            get => _pagina;
            set => _pagina = value > 0 ? value : 1;
        }

        public int RegistrosPorPagina
        {
            get => _registrosPorPagina;
            set => _registrosPorPagina = (value > 0 && value <= _maximoRegistrosPorPagina) 
                ? value : _registrosPorPagina;
        }

        public string OrdenarPor { get; set; } = "Id";
        public bool Ascendente { get; set; } = true;
    }

    public class ResultadoPaginadoDTO<T>
    {
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public string OrdenarPor { get; set; }
        public bool Ascendente { get; set; }
        public T[] Registros { get; set; }
    }
}