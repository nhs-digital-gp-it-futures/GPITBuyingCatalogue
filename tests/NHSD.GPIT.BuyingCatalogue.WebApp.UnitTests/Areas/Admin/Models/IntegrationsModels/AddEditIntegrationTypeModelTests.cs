using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
        model.IsReferenced.Should().Be(integrationType.Solutions.Count > 0);
    }

    [Theory]
    [MockInlineAutoData(SupportedIntegrations.Im1, null, false, false)]
    [MockInlineAutoData(null, 0, false, false)]
    [MockInlineAutoData(null, null, true, false)]
    [MockInlineAutoData(SupportedIntegrations.Im1, 0, false, false)]
    [MockInlineAutoData(null, 0, false, false)]
    [MockInlineAutoData(SupportedIntegrations.Im1, 0, true, true)]
    public static void ShouldShowFilterLink_ReturnsExpected(
        SupportedIntegrations? integrationId,
        int? integrationTypeId,
        bool isReferenced,
        bool expected)
    {
        var model = new AddEditIntegrationTypeModel
        {
            IntegrationId = integrationId,
            IntegrationTypeId = integrationTypeId,
            IsReferenced = isReferenced,
        };

        model.ShouldShowFilterLink.Should().Be(expected);
    }

    [Fact]
    public static void FilterQueryString_ReturnsExpected()
    {
        var model = new AddEditIntegrationTypeModel
        {
            IntegrationId = SupportedIntegrations.Im1,
            IntegrationTypeId = 5,
        };

        model.FilterQueryString.Should().Be("0.5");
    }
}
