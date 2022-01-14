using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    [ExcludeFromCodeCoverage]
    public static class ServicesStartup
    {
        private static readonly Regex AssemblyScanRegex = new(@"NHSD\.GPIT\.BuyingCatalogue(?!.*Test.*)", RegexOptions.Compiled);

        public static void Configure(IServiceCollection services)
        {
            ConfigureInterfaceClasses(services);
        }

        private static void ConfigureInterfaceClasses(IServiceCollection services)
        {
            services.Scan(ts => ts.FromAssemblies(AssembliesToScan())
                .AddClasses().AsMatchingInterface()
                .WithTransientLifetime());
        }

        private static IEnumerable<Assembly> AssembliesToScan()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => AssemblyScanRegex.IsMatch(assembly.FullName))
                .ToList();
        }
    }
}
