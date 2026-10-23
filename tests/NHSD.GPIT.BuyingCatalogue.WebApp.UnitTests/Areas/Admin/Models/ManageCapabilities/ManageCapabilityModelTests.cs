using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageCapabilities;

public static class ManageCapabilityModelTests
{
    [Fact]
    public static void Constructor_Capability_MapsExpectedProperties()
    {
        var expectedId = new CatalogueItemId(100001, "001");

        var capability = new Capability
        {
            Id = 42,
            CapabilityRef = "CAP-001",
            Name = "Capability name",
            Description = "Capability description",
            SourceUrl = "https://example.org/capability",
            Status = CapabilityStatus.Effective,
            CatalogueItemCapabilities = new List<CatalogueItemCapability>
            {
                new()
                {
                    CatalogueItem = new CatalogueItem
                    {
                        Id = expectedId,
                        Name = "Solution A",
                        Supplier = new Supplier { Name = "Supplier A" },
                        CatalogueItemType = CatalogueItemType.Solution,
                        PublishedStatus = PublicationStatus.Published,
                    },
                },
            },
        };

        var model = new ManageCapabilityModel(capability);

        model.Id.Should().Be(capability.Id);
        model.CapabilityRef.Should().Be(capability.CapabilityRef);
        model.Name.Should().Be(capability.Name);
        model.Description.Should().Be(capability.Description);
        model.SourceUrl.Should().Be(capability.SourceUrl);
        model.Status.Should().Be(capability.Status);

        var expectedCatalogueItem = new AdminManageCapabilityCatalogueItem
        {
            Id = expectedId,
            Name = "Solution A",
            SupplierName = "Supplier A",
            CatalogueItemType = CatalogueItemType.Solution,
            PublishedStatus = PublicationStatus.Published,
        };

        model.CatalogueItemsExpanderModel.Should().NotBeNull();
        model.CatalogueItemsExpanderModel.CatalogueItems.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(expectedCatalogueItem);
    }
}
