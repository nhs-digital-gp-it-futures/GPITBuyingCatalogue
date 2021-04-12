using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IHealthChecksBuilder AddHealthChecks(this IServiceCollection services, string connectionString)
        {
            if (connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));

            var healthChecksBuilder = services.AddHealthChecks();

            healthChecksBuilder.AddCheck(
                    "self",
                    () => HealthCheckResult.Healthy(),
                    new[] { HealthCheckTags.Live })
                .AddSqlServer(
                    connectionString,
                    "SELECT 1;",
                    "db",
                    HealthStatus.Unhealthy,
                    new[] { HealthCheckTags.Ready },
                    TimeSpan.FromSeconds(10));

            return healthChecksBuilder;
        }
    }
}
