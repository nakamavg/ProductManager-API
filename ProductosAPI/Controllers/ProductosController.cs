using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ProductosAPI.DTOs;
using ProductosAPI.Filters;
using ProductosAPI.Models;
using ProductosAPI.Services;

namespace ProductosAPI.Controllers
{
    [RoutePrefix("api/productos")]
    public class ProductosController : ApiController
    {
        private readonly ProductoService _productoService;

        public ProductosController()
        {
            _productoService = new ProductoService();
        }

        // GET api/productos?pagina=1&registrosPorPagina=10&ordenarPor=Nombre&ascendente=true
        [HttpGet]
        [Route("")]
        [JwtAuthorizationFilter] // Cualquier usuario autenticado puede acceder
        public async Task<IHttpActionResult> GetProductos([FromUri] PaginacionDTO paginacion = null)
        {
            try
            {
                paginacion = paginacion ?? new PaginacionDTO();
                var resultado = await _productoService.ObtenerProductosPaginadosAsync(paginacion);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/productos/5
        [HttpGet]
        [Route("{id:int}")]
        [JwtAuthorizationFilter] // Cualquier usuario autenticado puede acceder
        public async Task<IHttpActionResult> GetProducto(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerProductoPorIdAsync(id);
                
                if (producto == null)
                {
                    return NotFound();
                }
                
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST api/productos
        [HttpPost]
        [Route("")]
        [JwtAuthorizationFilter(Rol.Admin)] // Solo usuarios con rol Admin
        public async Task<IHttpActionResult> PostProducto(ProductoCreateDTO productoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var producto = await _productoService.CrearProductoAsync(productoDto);
                return CreatedAtRoute("DefaultApi", new { id = producto.Id }, producto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/productos/5
        [HttpPut]
        [Route("{id:int}")]
        [JwtAuthorizationFilter(Rol.Admin)] // Solo usuarios con rol Admin
        public async Task<IHttpActionResult> PutProducto(int id, ProductoUpdateDTO productoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var producto = await _productoService.ActualizarProductoAsync(id, productoDto);
                
                if (producto == null)
                {
                    return NotFound();
                }
                
                return Ok(producto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/productos/5
        [HttpDelete]
        [Route("{id:int}")]
        [JwtAuthorizationFilter(Rol.Admin)] // Solo usuarios con rol Admin
        public async Task<IHttpActionResult> DeleteProducto(int id)
        {
            try
            {
                var resultado = await _productoService.EliminarProductoAsync(id);
                
                if (!resultado)
                {
                    return NotFound();
                }
                
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}