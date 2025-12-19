using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SistemaGestaoCursos.HealthChecks
{
    public class CustomHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            if (isHealthy)
                return Task.FromResult(HealthCheckResult.Healthy("API está operacional"));

            return Task.FromResult(
                HealthCheckResult.Unhealthy("API está com problemas"));
        }
    }
}