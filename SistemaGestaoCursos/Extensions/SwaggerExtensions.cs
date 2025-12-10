using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SistemaGestaoCursos.Filters;

namespace SistemaGestaoCursos.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddCustomSwagger(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sistema Gestão Cursos API",
                    Version = "v1",
                    Description = "API para gerenciamento de cursos, alunos e matrículas",
                    Contact = new OpenApiContact
                    {
                        Name = "Suporte",
                        Email = "suporte@sistemacursos.com"
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT no formato: Bearer {seu_token}"
                });

                options.OperationFilter<AddCorrelationIdHeaderFilter>();

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

               

                options.OrderActionsBy(apiDesc => $"{apiDesc.HttpMethod} {apiDesc.RelativePath}");
            });

            return services;
        }
    }
}