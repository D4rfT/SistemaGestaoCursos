using FluentValidation;
using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SistemaGestaoCursos.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Erro não tratado capturado pelo middleware");

            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                KeyNotFoundException e => (HttpStatusCode.NotFound, e.Message),

                InvalidOperationException e => (HttpStatusCode.BadRequest, e.Message),

                ArgumentException e => (HttpStatusCode.BadRequest, e.Message),

                UnauthorizedAccessException e => (HttpStatusCode.Unauthorized, "Acesso não autorizado"),

                _ => (HttpStatusCode.InternalServerError, _env.IsDevelopment()? exception.Message : "Ocorreu um erro interno. Tente novamente mais tarde.")
            };

            context.Response.StatusCode = (int)statusCode;

            var errors = exception is ValidationException validationEx
                ? validationEx.Errors.Select(e => e.ErrorMessage).ToList()
                : null;

            var response = new
            {
                statusCode = (int)statusCode,
                message,
                errors,
                details = _env.IsDevelopment() ? exception.StackTrace : null,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
    }
}