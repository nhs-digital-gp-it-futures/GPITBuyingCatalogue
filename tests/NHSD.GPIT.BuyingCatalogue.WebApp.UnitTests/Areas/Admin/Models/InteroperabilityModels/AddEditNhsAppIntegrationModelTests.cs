using System.Linq;
using System.Text.Json;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.InteroperabilityModels;

public static class AddEditNhsAppIntegrationModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution)
    {
        var model = new AddEditNhsAppIntegrationModel(solution.CatalogueItem);

        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_WithIntegrations_SetsAsExpected(
        Solution solution)
    {
        var integrations = Interoperability.NhsAppIntegrations
            .Select(x => new Integration(Interoperability.NhsAppIntegrationType, x))
            .ToList();

        var expectedIntegrations = integrations.Select(x => new SelectOption<string>(x.Qualifier, x.Qualifier, true));

        solution.Integrations = JsonSerializer.Serialize(integrations);

        var model = new AddEditNhsAppIntegrationModel(solution.CatalogueItem);

        model.NhsAppIntegrations.Should().BeEquivalentTo(expectedIntegrations);
    }
}
