using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductosAPI.Core.DTOs;
using ProductosAPI.Core.Models;
using ProductosAPI.Core.Services;

namespace ProductosAPI.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoService _productoService;

        public ProductosController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<ActionResult<ResultadoPaginadoDTO<ProductoDTO>>> GetProductos([FromQuery] PaginacionDTO paginacion)
        {
            var resultado = await _productoService.ObtenerProductosPaginadosAsync(paginacion);
            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            
            if (producto == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            return Ok(producto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductoDTO>> PostProducto(ProductoCreateDTO productoDto)
        {
            try
            {
                var producto = await _productoService.CrearProductoAsync(productoDto);
                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductoDTO>> PutProducto(int id, ProductoUpdateDTO productoDto)
        {
            try
            {
                var producto = await _productoService.ActualizarProductoAsync(id, productoDto);
                
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                return Ok(producto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            var resultado = await _productoService.EliminarProductoAsync(id);
            
            if (!resultado)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            return NoContent();
        }
    }
}