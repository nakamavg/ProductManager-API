using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ProductosAPI.Data;
using ProductosAPI.DTOs;
using ProductosAPI.Models;

namespace ProductosAPI.Services
{
    public class ProductoService
    {
        private readonly ProductosDbContext _context;
        private readonly AuditService _auditService;

        public ProductoService()
        {
            _context = new ProductosDbContext();
            _auditService = new AuditService();
        }

        public async Task<ResultadoPaginadoDTO<ProductoDTO>> ObtenerProductosPaginadosAsync(PaginacionDTO paginacion)
        {
            // Filtrar solo productos activos
            var query = _context.Productos.Where(p => p.Activo);

            // Ordenar según el parámetro
            string ordenarPor = string.IsNullOrEmpty(paginacion.OrdenarPor) ? "Id" : paginacion.OrdenarPor;
            query = paginacion.Ascendente ? 
                query.OrderBy(ordenarPor) : 
                query.OrderBy($"{ordenarPor} descending");

            // Calcular total
            int totalRegistros = await query.CountAsync();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)paginacion.RegistrosPorPagina);

            // Aplicar paginación
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
                .ToArrayAsync();

            return new ResultadoPaginadoDTO<ProductoDTO>
            {
                PaginaActual = paginacion.Pagina,
                RegistrosPorPagina = paginacion.RegistrosPorPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                OrdenarPor = ordenarPor,
                Ascendente = paginacion.Ascendente,
                Registros = productos
            };
        }

        public async Task<ProductoDTO> ObtenerProductoPorIdAsync(int id)
        {
            var producto = await _context.Productos
                .Where(p => p.Id == id && p.Activo)
                .FirstOrDefaultAsync();

            if (producto == null)
                return null;

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

        public async Task<ProductoDTO> CrearProductoAsync(ProductoCreateDTO productoDTO)
        {
            // Verificar si ya existe un producto con el mismo nombre
            if (await _context.Productos.AnyAsync(p => p.Nombre == productoDTO.Nombre && p.Activo))
            {
                throw new InvalidOperationException("Ya existe un producto con este nombre.");
            }

            // Calcular volumen
            decimal volumen = productoDTO.Alto * productoDTO.Ancho * productoDTO.Profundidad;

            // Crear el nuevo producto
            var producto = new Producto
            {
                Nombre = productoDTO.Nombre,
                Alto = productoDTO.Alto,
                Ancho = productoDTO.Ancho,
                Profundidad = productoDTO.Profundidad,
                Volumen = volumen,
                Peso = productoDTO.Peso,
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

        public async Task<ProductoDTO> ActualizarProductoAsync(int id, ProductoUpdateDTO productoDTO)
        {
            var producto = await _context.Productos
                .Where(p => p.Id == id && p.Activo)
                .FirstOrDefaultAsync();

            if (producto == null)
                return null;

            // Verificar si ya existe otro producto con el mismo nombre
            if (await _context.Productos.AnyAsync(p => p.Nombre == productoDTO.Nombre && p.Id != id && p.Activo))
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
            producto.Nombre = productoDTO.Nombre;
            producto.Alto = productoDTO.Alto;
            producto.Ancho = productoDTO.Ancho;
            producto.Profundidad = productoDTO.Profundidad;
            producto.Volumen = productoDTO.Alto * productoDTO.Ancho * productoDTO.Profundidad;
            producto.Peso = productoDTO.Peso;

            await _context.SaveChangesAsync();

            // Registrar auditoría
            await _auditService.RegistrarAuditoria("Actualizar", "Producto", producto.Id, 
                productoAnterior, producto);

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

            if (producto == null)
                return false;

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

            // Realizar eliminación lógica
            producto.Activo = false;
            await _context.SaveChangesAsync();

            // Registrar auditoría
            await _auditService.RegistrarAuditoria("Eliminar", "Producto", producto.Id, 
                productoAnterior, producto);

            return true;
        }
    }
}