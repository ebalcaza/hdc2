using backend.BLL.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Middleware;

/// <summary>Convierte las excepciones de negocio en respuestas HTTP con formato ProblemDetails.</summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (status, titulo) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "No encontrado"),
                UnauthorizedBusinessException => (StatusCodes.Status401Unauthorized, "No autorizado"),
                BusinessRuleException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Error no controlado en {Metodo} {Ruta}", context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted) throw;

            context.Response.Clear();
            context.Response.StatusCode = status;

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado."
                    : ex.Message
            });
        }
    }
}
