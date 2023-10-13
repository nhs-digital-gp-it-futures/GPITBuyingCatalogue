using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.InteroperabilityModels
{
    public static class AddEditNhsAppIntegrationModelTests
    {
        [Fact]
        public static void CheckIntegrationTypes_WithNullIntegrations_DoesNotSetCheckedProperties()
        {
            var model = new AddEditNhsAppIntegrationModel();
            model.SetIntegrationTypes();

            var integrations = (ICollection<Integration>)null;

            model.CheckIntegrationTypes(integrations);

            model.NhsAppIntegrationTypes.Should().NotBeNull(); // Make sure it's not null
            model.NhsAppIntegrationTypes.Should().OnlyContain(type => !type.Checked);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task CheckIntegrationTypes_WithIntegrations_DoesNotSetCheckedProperties(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<Integration> integrations)
        {
            var model = new AddEditNhsAppIntegrationModel();
            model.SetIntegrationTypes();
            integrations = new()
            {
                new Integration
                {
                    Id = Guid.Parse("12345678-1234-5678-1234-567812345678"),
                    IntegrationType = NHSD.GPIT.BuyingCatalogue.Framework.Constants.Interoperability.NhsAppIntegrationType,
                    NHSAppIntegrationTypes = model.NhsAppIntegrationTypes
                            .Where(type => type.Checked)
                            .Select(type => type.IntegrationType)
                            .ToHashSet(),
                },
            };
            solution.Integrations = JsonSerializer.Serialize(integrations);
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();

            var solutionIntegrations = solution.GetIntegrations();
            model.CheckIntegrationTypes(solutionIntegrations);

            model.NhsAppIntegrationTypes.Should().NotBeNull();
            model.NhsAppIntegrationTypes.Should().OnlyContain(type => !type.Checked);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task CheckIntegrationTypes_WithIntegrations_DoesSetCheckedProperties(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<Integration> integrations)
        {
            var model = new AddEditNhsAppIntegrationModel();
            model.SetIntegrationTypes();
            integrations = new()
            {
                new Integration
                {
                    Id = Guid.Parse("12345678-1234-5678-1234-567812345678"),
                    IntegrationType = NHSD.GPIT.BuyingCatalogue.Framework.Constants.Interoperability.NhsAppIntegrationType,
                    NHSAppIntegrationTypes = model.NhsAppIntegrationTypes
                            .Select(type => type.IntegrationType)
                            .ToHashSet(),
                },
            };
            solution.Integrations = JsonSerializer.Serialize(integrations);
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();

            var solutionIntegrations = solution.GetIntegrations();
            model.CheckIntegrationTypes(solutionIntegrations);

            model.NhsAppIntegrationTypes.Should().NotBeNull();
            model.NhsAppIntegrationTypes.Should().OnlyContain(type => type.Checked);
        }

        [Fact]
        public static void CheckIntegrationTypes_WhenIntegrationsIsNull_ShouldNotThrowException()
        {
            var model = new AddEditNhsAppIntegrationModel();

            Action action = () => model.CheckIntegrationTypes(null);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public static void CheckIntegrationTypes_WhenNhsAppIntegrationNotFound_ShouldNotChangeIntegrationIdAndCheckedProperties()
        {
            var model = new AddEditNhsAppIntegrationModel();
            var integrations = new List<Integration>();

            model.CheckIntegrationTypes(integrations);

            model.IntegrationId.Should().Be(Guid.Empty);
            model.NhsAppIntegrationTypes.Should().BeNull();
        }
    }
}
