using System.Security.Claims;
using Newtonsoft.Json;
using ProductosAPI.Core.Data;
using ProductosAPI.Core.Models;

namespace ProductosAPI.Core.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RegistrarAuditoria<T>(string accion, string entidad, int entidadId, 
            T? valorAnterior = default, T? valorNuevo = default)
        {
            try
            {
                var userId = ObtenerUsuarioActualId();
                
                var log = new AuditLog
                {
                    Accion = accion,
                    Entidad = entidad,
                    EntidadId = entidadId,
                    ValorAnterior = valorAnterior != null ? JsonConvert.SerializeObject(valorAnterior) : null,
                    ValorNuevo = valorNuevo != null ? JsonConvert.SerializeObject(valorNuevo) : null,
                    UsuarioId = userId,
                    FechaAccion = DateTime.Now,
                    DireccionIP = ObtenerDireccionIP()
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // En un entorno de producción, aquí deberíamos usar un sistema de logging apropiado
                Console.WriteLine($"Error al registrar auditoría: {ex.Message}");
            }
        }

        private int ObtenerUsuarioActualId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) ? userId : 0;
        }

        private string ObtenerDireccionIP()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return "Unknown";

            var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}