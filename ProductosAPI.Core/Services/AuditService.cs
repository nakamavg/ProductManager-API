using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductosAPI.Core.Data;
using ProductosAPI.Core.Models;

namespace ProductosAPI.Core.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _logPath;

        public AuditService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logPath = configuration["AuditLogPath"] ?? "logs/audit";
            
            // Asegurar que existe el directorio de logs
            Directory.CreateDirectory(_logPath);
        }

        public async Task RegistrarAuditoria<T>(string accion, string entidad, int entidadId, 
            T? valorAnterior = default, T? valorNuevo = default)
        {
            try
            {
                var userId = ObtenerUsuarioActualId();
                var ip = ObtenerDireccionIP();
                var userAgent = ObtenerUserAgent();
                var timestamp = DateTime.UtcNow;
                
                var log = new AuditLog
                {
                    Accion = accion,
                    Entidad = entidad,
                    EntidadId = entidadId,
                    ValorAnterior = valorAnterior != null ? JsonConvert.SerializeObject(valorAnterior) : null,
                    ValorNuevo = valorNuevo != null ? JsonConvert.SerializeObject(valorNuevo) : null,
                    UsuarioId = userId,
                    FechaAccion = timestamp,
                    DireccionIP = ip,
                    UserAgent = userAgent,
                    Detalles = GenerarDetallesAdicionales(accion, entidad, valorAnterior, valorNuevo)
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();

                // Guardar también en archivo
                await GuardarEnArchivo(log);
            }
            catch (Exception ex)
            {
                // Log de error pero permitir que la operación principal continúe
                Console.WriteLine($"Error al registrar auditoría: {ex.Message}");
            }
        }

        public async Task<List<AuditLog>> ObtenerHistorialPorEntidad(string entidad, int entidadId)
        {
            return await _context.AuditLogs
                .Where(l => l.Entidad == entidad && l.EntidadId == entidadId)
                .OrderByDescending(l => l.FechaAccion)
                .ToListAsync();
        }

        public async Task<byte[]> ExportarAuditoria(DateTime? desde = null, DateTime? hasta = null)
        {
            var query = _context.AuditLogs.AsQueryable();
            
            if (desde.HasValue)
                query = query.Where(l => l.FechaAccion >= desde.Value);
            
            if (hasta.HasValue)
                query = query.Where(l => l.FechaAccion <= hasta.Value);

            var logs = await query.OrderByDescending(l => l.FechaAccion).ToListAsync();
            var csv = new StringBuilder();
            
            csv.AppendLine("Fecha,Acción,Entidad,EntidadId,Usuario,IP,Detalles");
            
            foreach (var log in logs)
            {
                csv.AppendLine($"{log.FechaAccion:yyyy-MM-dd HH:mm:ss}," +
                             $"{log.Accion}," +
                             $"{log.Entidad}," +
                             $"{log.EntidadId}," +
                             $"{log.UsuarioId}," +
                             $"{log.DireccionIP}," +
                             $"\"{log.Detalles?.Replace("\"", "\"\"")}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private async Task GuardarEnArchivo(AuditLog log)
        {
            var logFileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
            var logFilePath = Path.Combine(_logPath, logFileName);
            
            var logEntry = JsonConvert.SerializeObject(log, Formatting.Indented);
            await File.AppendAllTextAsync(logFilePath, logEntry + Environment.NewLine);
        }

        private int ObtenerUsuarioActualId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub");
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) ? userId : 0;
        }

        private string ObtenerDireccionIP()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return "Unknown";

            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private string ObtenerUserAgent()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
        }

        private string GenerarDetallesAdicionales<T>(string accion, string entidad, T? valorAnterior, T? valorNuevo)
        {
            var detalles = new StringBuilder();
            
            switch (accion.ToLower())
            {
                case "crear":
                    detalles.Append($"Creación de nuevo {entidad}");
                    break;
                    
                case "actualizar":
                    detalles.Append($"Actualización de {entidad}");
                    if (valorAnterior != null && valorNuevo != null)
                    {
                        var cambios = CompararObjetos(valorAnterior, valorNuevo);
                        if (cambios.Any())
                        {
                            detalles.Append(". Campos modificados: ");
                            detalles.Append(string.Join(", ", cambios));
                        }
                    }
                    break;
                    
                case "eliminar":
                    detalles.Append($"Eliminación de {entidad}");
                    break;
            }

            return detalles.ToString();
        }

        private List<string> CompararObjetos<T>(T anterior, T nuevo)
        {
            var cambios = new List<string>();
            var propiedades = typeof(T).GetProperties();

            foreach (var propiedad in propiedades)
            {
                var valorAnterior = propiedad.GetValue(anterior)?.ToString();
                var valorNuevo = propiedad.GetValue(nuevo)?.ToString();

                if (valorAnterior != valorNuevo)
                {
                    cambios.Add($"{propiedad.Name}: {valorAnterior} -> {valorNuevo}");
                }
            }

            return cambios;
        }
    }
}