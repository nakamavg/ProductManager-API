using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using ProductosAPI.Data;
using ProductosAPI.Models;

namespace ProductosAPI.Services
{
    public class AuditService
    {
        private readonly ProductosDbContext _context;

        public AuditService()
        {
            _context = new ProductosDbContext();
        }

        public async Task RegistrarAuditoria<T>(string accion, string entidad, int entidadId, 
            T valorAnterior = default, T valorNuevo = default)
        {
            try
            {
                int usuarioId = ObtenerUsuarioActualId();
                string anterior = valorAnterior != null ? JsonConvert.SerializeObject(valorAnterior) : null;
                string nuevo = valorNuevo != null ? JsonConvert.SerializeObject(valorNuevo) : null;

                var log = new AuditLog
                {
                    Accion = accion,
                    Entidad = entidad,
                    EntidadId = entidadId,
                    ValorAnterior = anterior,
                    ValorNuevo = nuevo,
                    UsuarioId = usuarioId,
                    FechaAccion = DateTime.Now,
                    DireccionIP = ObtenerDireccionIP()
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // En un entorno de producción, loguear el error
                System.Diagnostics.Debug.WriteLine($"Error al registrar auditoría: {ex.Message}");
            }
        }

        private int ObtenerUsuarioActualId()
        {
            try
            {
                var identity = Thread.CurrentPrincipal?.Identity as ClaimsIdentity;
                if (identity == null)
                    return 0;

                var claim = identity.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
                if (claim == null)
                    return 0;

                return int.TryParse(claim.Value, out int id) ? id : 0;
            }
            catch
            {
                return 0;
            }
        }

        private string ObtenerDireccionIP()
        {
            try
            {
                var request = HttpContext.Current?.Request;
                if (request == null)
                    return "Unknown";

                string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ip))
                {
                    ip = request.ServerVariables["REMOTE_ADDR"];
                }

                return ip ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}