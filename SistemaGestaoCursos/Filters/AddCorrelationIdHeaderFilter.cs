using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SistemaGestaoCursos.Filters
{
    public class AddCorrelationIdHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            var correlationIdParam = operation.Parameters.FirstOrDefault(p => p.Name.Equals("X-Correlation-ID", StringComparison.OrdinalIgnoreCase));

            if (correlationIdParam == null)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Correlation-ID",
                    In = ParameterLocation.Header,
                    Description = "ID único para rastreamento da requisição. Se não fornecido, será gerado automaticamente.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" },
                    Example = new OpenApiString(Guid.NewGuid().ToString())
                });
            }
        }
    }
}