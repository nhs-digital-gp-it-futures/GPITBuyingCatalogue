using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacyCapabilitiesModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_FiltersExpiredCapabilitiesAndMapsRows(
        Solution solution,
        CatalogueItemCapability effectiveCapability,
        CatalogueItemCapability expiredCapability,
        CatalogueItemContentStatus contentStatus)
    {
        effectiveCapability.Capability.Status = CapabilityStatus.Effective;
        expiredCapability.Capability.Status = CapabilityStatus.Expired;
        solution.CatalogueItem.CatalogueItemCapabilities = new List<CatalogueItemCapability>
        {
            effectiveCapability, expiredCapability,
        };

        var model = new CommunityPharmacyCapabilitiesModel(solution.CatalogueItem, contentStatus);

        model.RowViewModels.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new RowViewModel(effectiveCapability));
        model.CapabilitiesHeading.Should().Be("Capabilities met");
        model.SelectedSection.Should().Be("Capabilities");
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NoCapabilities_ReturnsEmptyRows(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.CatalogueItem.CatalogueItemCapabilities.Clear();

        var model = new CommunityPharmacyCapabilitiesModel(solution.CatalogueItem, contentStatus);

        model.RowViewModels.Should().BeEmpty();
    }
}
