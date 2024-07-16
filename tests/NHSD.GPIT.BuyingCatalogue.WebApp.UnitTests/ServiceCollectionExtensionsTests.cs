using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Theory]
        [MockAutoData]
        public static void AddDataProtection_WithAppName_AddsDataProtection(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.ConfigureDataProtection(configuration);

            using var provider = services.BuildServiceProvider();
            var dpProvider = provider.GetRequiredService<IDataProtectionProvider>();

            dpProvider.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void AddDataProtection_WithAppName_SetsAppDiscriminator(IConfiguration configuration)
        {
            var appName = configuration.GetValue<string>("dataProtection:applicationName");

            var services = new ServiceCollection();

            services.ConfigureDataProtection(configuration);

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<DataProtectionOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.ApplicationDiscriminator.Should().Be(appName);
        }

        [Theory]
        [MockAutoData]
        public static void AddDataProtection_WithAppName_SetsXmlRepository(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.ConfigureDataProtection(configuration);

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.XmlRepository.Should().NotBeNull();
            options.Value.XmlRepository.Should().BeOfType<EntityFrameworkCoreXmlRepository<BuyingCatalogueDbContext>>();
        }

        [Theory]
        [MockAutoData]
        public static void AddDataProtection_WithAppName_DoesNotSetXmlEncryptor(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.ConfigureDataProtection(configuration);

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.XmlEncryptor.Should().BeNull();
        }
    }
}
