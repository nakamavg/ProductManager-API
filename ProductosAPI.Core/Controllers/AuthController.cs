using Microsoft.AspNetCore.Mvc;
using ProductosAPI.Core.DTOs;
using ProductosAPI.Core.Services;

namespace ProductosAPI.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public AuthController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDTO registerDto)
        {
            try
            {
                var usuario = await _usuarioService.RegistrarUsuarioAsync(registerDto);
                return Ok(new
                {
                    mensaje = "Usuario registrado correctamente",
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    email = usuario.Email,
                    rol = usuario.Rol
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDTO loginDto)
        {
            var resultado = await _usuarioService.AutenticarUsuarioAsync(loginDto);
            
            if (resultado == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(resultado);
        }
    }
}