using Microsoft.EntityFrameworkCore;
using ProductosAPI.Core.Data;
using ProductosAPI.Core.DTOs;
using ProductosAPI.Core.Models;

namespace ProductosAPI.Core.Services
{
    public class ProductoService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;

        public ProductoService(AppDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<ResultadoPaginadoDTO<ProductoDTO>> ObtenerProductosPaginadosAsync(PaginacionDTO paginacion)
        {
            // Filtrar solo productos activos
            var query = _context.Productos.Where(p => p.Activo);

            // Aplicar ordenamiento
            query = paginacion.OrdenarPor.ToLower() switch
            {
                "nombre" => paginacion.Ascendente ? 
                    query.OrderBy(p => p.Nombre) : 
                    query.OrderByDescending(p => p.Nombre),
                "fechacreacion" => paginacion.Ascendente ? 
                    query.OrderBy(p => p.FechaCreacion) : 
                    query.OrderByDescending(p => p.FechaCreacion),
                _ => paginacion.Ascendente ? 
                    query.OrderBy(p => p.Id) : 
                    query.OrderByDescending(p => p.Id)
            };

            // Obtener total de registros
            var totalRegistros = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)paginacion.RegistrosPorPagina);

            // Obtener página específica
            var productos = await query
                .Skip((paginacion.Pagina - 1) * paginacion.RegistrosPorPagina)
                .Take(paginacion.RegistrosPorPagina)
                .Select(p => new ProductoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Alto = p.Alto,
                    Ancho = p.Ancho,
                    Profundidad = p.Profundidad,
                    Volumen = p.Volumen,
                    Peso = p.Peso,
                    FechaCreacion = p.FechaCreacion,
                    Activo = p.Activo
                })
                .ToListAsync();

            return new ResultadoPaginadoDTO<ProductoDTO>
            {
                PaginaActual = paginacion.Pagina,
                RegistrosPorPagina = paginacion.RegistrosPorPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                OrdenarPor = paginacion.OrdenarPor,
                Ascendente = paginacion.Ascendente,
                Registros = productos
            };
        }

        public async Task<ProductoDTO?> ObtenerProductoPorIdAsync(int id)
        {
            var producto = await _context.Productos
                .Where(p => p.Id == id && p.Activo)
                .Select(p => new ProductoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Alto = p.Alto,
                    Ancho = p.Ancho,
                    Profundidad = p.Profundidad,
                    Volumen = p.Volumen,
                    Peso = p.Peso,
                    FechaCreacion = p.FechaCreacion,
                    Activo = p.Activo
                })
                .FirstOrDefaultAsync();

            return producto;
        }

        public async Task<ProductoDTO> CrearProductoAsync(ProductoCreateDTO productoDto)
        {
            // Verificar nombre único
            if (await _context.Productos.AnyAsync(p => p.Nombre == productoDto.Nombre && p.Activo))
            {
                throw new InvalidOperationException("Ya existe un producto con este nombre.");
            }

            // Calcular volumen
            var volumen = productoDto.Alto * productoDto.Ancho * productoDto.Profundidad;

            var producto = new Producto
            {
                Nombre = productoDto.Nombre,
                Alto = productoDto.Alto,
                Ancho = productoDto.Ancho,
                Profundidad = productoDto.Profundidad,
                Volumen = volumen,
                Peso = productoDto.Peso,
                FechaCreacion = DateTime.Now,
                Activo = true
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Registrar auditoría
            await _auditService.RegistrarAuditoria("Crear", "Producto", producto.Id, null, producto);

            return new ProductoDTO
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Alto = producto.Alto,
                Ancho = producto.Ancho,
                Profundidad = producto.Profundidad,
                Volumen = producto.Volumen,
                Peso = producto.Peso,
                FechaCreacion = producto.FechaCreacion,
                Activo = producto.Activo
            };
        }

        public async Task<ProductoDTO?> ActualizarProductoAsync(int id, ProductoUpdateDTO productoDto)
        {
            var producto = await _context.Productos
                .Where(p => p.Id == id && p.Activo)
                .FirstOrDefaultAsync();

            if (producto == null) return null;

            // Verificar nombre único
            if (producto.Nombre != productoDto.Nombre &&
                await _context.Productos.AnyAsync(p => p.Nombre == productoDto.Nombre && p.Activo && p.Id != id))
            {
                throw new InvalidOperationException("Ya existe otro producto con este nombre.");
            }

            // Guardar estado anterior para auditoría
            var productoAnterior = new Producto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Alto = producto.Alto,
                Ancho = producto.Ancho,
                Profundidad = producto.Profundidad,
                Volumen = producto.Volumen,
                Peso = producto.Peso,
                FechaCreacion = producto.FechaCreacion,
                Activo = producto.Activo
            };

            // Actualizar propiedades
            producto.Nombre = productoDto.Nombre;
            producto.Alto = productoDto.Alto;
            producto.Ancho = productoDto.Ancho;
            producto.Profundidad = productoDto.Profundidad;
            producto.Volumen = productoDto.Alto * productoDto.Ancho * productoDto.Profundidad;
            producto.Peso = productoDto.Peso;

            await _context.SaveChangesAsync();

            // Registrar auditoría
            await _auditService.RegistrarAuditoria("Actualizar", "Producto", producto.Id, productoAnterior, producto);

            return new ProductoDTO
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Alto = producto.Alto,
                Ancho = producto.Ancho,
                Profundidad = producto.Profundidad,
                Volumen = producto.Volumen,
                Peso = producto.Peso,
                FechaCreacion = producto.FechaCreacion,
                Activo = producto.Activo
            };
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            var producto = await _context.Productos
                .Where(p => p.Id == id && p.Activo)
                .FirstOrDefaultAsync();

            if (producto == null) return false;

            // Guardar estado anterior para auditoría
            var productoAnterior = new Producto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Alto = producto.Alto,
                Ancho = producto.Ancho,
                Profundidad = producto.Profundidad,
                Volumen = producto.Volumen,
                Peso = producto.Peso,
                FechaCreacion = producto.FechaCreacion,
                Activo = true
            };

            // Eliminar lógicamente
            producto.Activo = false;
            await _context.SaveChangesAsync();

            // Registrar auditoría
            await _auditService.RegistrarAuditoria("Eliminar", "Producto", producto.Id, productoAnterior, producto);

            return true;
        }
    }
}