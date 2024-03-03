using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace ProductosAPI.Filters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string errorMessage = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            // Personalizar el manejo de excepciones según el tipo
            if (actionExecutedContext.Exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                errorMessage = "No está autorizado para realizar esta acción.";
            }
            else if (actionExecutedContext.Exception is ArgumentNullException || 
                     actionExecutedContext.Exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
                errorMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                // Log de error para excepciones no controladas
                errorMessage = "Ha ocurrido un error inesperado. Por favor, contacte al administrador.";
                
                // En producción, podríamos registrar el error en un sistema de logs
                System.Diagnostics.Debug.WriteLine($"ERROR: {actionExecutedContext.Exception.Message}");
            }

            // Crear una respuesta de error estandarizada
            actionExecutedContext.Response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(errorMessage),
                ReasonPhrase = errorMessage
            };
        }
    }
}