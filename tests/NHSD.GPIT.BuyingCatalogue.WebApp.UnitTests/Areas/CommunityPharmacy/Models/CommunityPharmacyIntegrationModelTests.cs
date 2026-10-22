using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacyIntegrationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_GroupsIntegrationsBySupportedIntegration(
        Solution solution,
        SolutionIntegration im1,
        SolutionIntegration gpConnect,
        SolutionIntegration nhsApp,
        CatalogueItemContentStatus contentStatus)
    {
        im1.IntegrationType = new IntegrationType { IntegrationId = SupportedIntegrations.Im1 };
        gpConnect.IntegrationType = new IntegrationType { IntegrationId = SupportedIntegrations.GpConnect };
        nhsApp.IntegrationType = new IntegrationType { IntegrationId = SupportedIntegrations.NhsApp };
        solution.Integrations = new List<SolutionIntegration> { im1, gpConnect, nhsApp };

        var model = new CommunityPharmacyIntegrationModel(solution.CatalogueItem, contentStatus);

        model.IM1Integrations.Should().Equal(im1);
        model.GpConnectIntegrations.Should().Equal(gpConnect);
        model.NhsAppIntegrations.Should().Equal(nhsApp);
        model.IntegrationsUrl.Should().Be(solution.IntegrationsUrl);
        model.SelectedSection.Should().Be("Integrations");
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NoIntegrations_ReturnsEmptyGroups(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.Integrations.Clear();

        var model = new CommunityPharmacyIntegrationModel(solution.CatalogueItem, contentStatus);

        model.IM1Integrations.Should().BeEmpty();
        model.GpConnectIntegrations.Should().BeEmpty();
        model.NhsAppIntegrations.Should().BeEmpty();
    }
}
