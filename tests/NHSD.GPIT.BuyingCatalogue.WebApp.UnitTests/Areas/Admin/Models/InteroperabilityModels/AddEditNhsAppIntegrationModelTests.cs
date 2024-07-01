using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.InteroperabilityModels;

public static class AddEditNhsAppIntegrationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution,
        List<IntegrationType> integrationTypes)
    {
        var model = new AddEditNhsAppIntegrationModel(solution.CatalogueItem, integrationTypes);

        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.NhsAppIntegrations.Should().HaveCount(integrationTypes.Count);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithIntegrations_SetsAsExpected(
        Solution solution,
        List<IntegrationType> integrationTypes)
    {
        solution.Integrations = integrationTypes.Select(x => new SolutionIntegration { IntegrationTypeId = x.Id }).ToList();

        var expectedIntegrations = integrationTypes.Select(x => new SelectOption<int>(x.Name, x.Id, true));

        var model = new AddEditNhsAppIntegrationModel(solution.CatalogueItem, integrationTypes);

        model.NhsAppIntegrations.Should().BeEquivalentTo(expectedIntegrations);
    }
}
