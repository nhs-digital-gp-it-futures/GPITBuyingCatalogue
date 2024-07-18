using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.IntegrationsModels;

public static class ViewIntegrationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Integration integration,
        List<IntegrationType> integrationTypes)
    {
        integration.IntegrationTypes = integrationTypes;

        var model = new ViewIntegrationModel(integration);

        model.IntegrationId.Should().Be(integration.Id);
        model.IntegrationName.Should().Be(integration.Name);
        model.IntegrationTypes.Should().BeEquivalentTo(integration.IntegrationTypes);
    }
}
