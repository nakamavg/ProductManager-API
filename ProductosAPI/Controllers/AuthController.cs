using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ProductosAPI.Auth;
using ProductosAPI.DTOs;
using ProductosAPI.Services;

namespace ProductosAPI.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly UsuarioService _usuarioService;
        private readonly JwtAuthService _jwtAuthService;

        public AuthController()
        {
            _usuarioService = new UsuarioService();
            _jwtAuthService = new JwtAuthService();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register(RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var usuario = await _usuarioService.RegistrarUsuarioAsync(registerDto);
                
                return Ok(new { 
                    mensaje = "Usuario registrado correctamente",
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    email = usuario.Email,
                    rol = usuario.Rol
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var usuario = await _usuarioService.AutenticarUsuarioAsync(loginDto.Email, loginDto.Password);
                
                if (usuario == null)
                {
                    return Content(HttpStatusCode.Unauthorized, "Credenciales inválidas");
                }

                var token = _jwtAuthService.GenerateJwtToken(usuario);
                int expiresIn = _jwtAuthService.GetExpiresInMinutes() * 60; // En segundos
                
                return Ok(new TokenResponseDTO
                {
                    Token = token,
                    ExpiresIn = expiresIn,
                    UserId = usuario.Id,
                    UserName = usuario.Nombre,
                    UserEmail = usuario.Email,
                    Rol = usuario.Rol
                });
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}