using System.Web.Http;
using System.Web.Http.Cors;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using ProductosAPI.Filters;

namespace ProductosAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración de CORS
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Configurar rutas de la API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configurar formato JSON camelCase
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Añadir filtro global para el manejo de excepciones
            config.Filters.Add(new GlobalExceptionFilter());
        }
    }
}