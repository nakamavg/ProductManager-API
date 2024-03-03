using System.ComponentModel.DataAnnotations;

namespace ProductosAPI.Core.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Alto { get; set; }
        public decimal Ancho { get; set; }
        public decimal Profundidad { get; set; }
        public decimal Volumen { get; set; }
        public decimal Peso { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }

    public class ProductoCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;
        
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
        public string Nombre { get; set; } = null!;
        
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
        private const int MaxRegistrosPorPagina = 50;

        public int Pagina
        {
            get => _pagina;
            set => _pagina = value > 0 ? value : 1;
        }

        public int RegistrosPorPagina
        {
            get => _registrosPorPagina;
            set => _registrosPorPagina = (value > 0 && value <= MaxRegistrosPorPagina) 
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
        public string OrdenarPor { get; set; } = null!;
        public bool Ascendente { get; set; }
        public IEnumerable<T> Registros { get; set; } = null!;
    }
}