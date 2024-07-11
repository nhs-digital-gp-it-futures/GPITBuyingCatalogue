using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.IntegrationsModels;

public static class AddEditIntegrationTypeModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_Integration_SetsPropertiesAsExpected(
        Integration integration)
    {
        var model = new AddEditIntegrationTypeModel(integration);

        model.IntegrationId.Should().Be(integration.Id);
        model.IntegrationName.Should().Be(integration.Name);
        model.RequiresDescription.Should().Be(integration.RequiresDescription);
        model.Title.Should().Be(AddEditIntegrationTypeModel.AddTitle);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_IntegrationType_SetsPropertiesAsExpected(
        Integration integration,
        IntegrationType integrationType)
    {
        var model = new AddEditIntegrationTypeModel(integration, integrationType);

        model.IntegrationTypeId.Should().Be(integrationType.Id);
        model.IntegrationTypeName.Should().Be(integrationType.Name);
        model.Description.Should().Be(integrationType.Description);
        model.Title.Should().Be(AddEditIntegrationTypeModel.EditTitle);
    }
}
