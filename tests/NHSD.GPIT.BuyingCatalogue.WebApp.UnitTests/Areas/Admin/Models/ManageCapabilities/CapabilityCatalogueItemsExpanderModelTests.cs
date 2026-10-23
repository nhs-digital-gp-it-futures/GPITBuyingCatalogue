using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageCapabilities;

public static class CapabilityCatalogueItemsExpanderModelTests
{
    [Fact]
    public static void CatalogueItems_SetValue_ReturnsExpectedValue()
    {
        IEnumerable<AdminManageCapabilityCatalogueItem> expected = new List<AdminManageCapabilityCatalogueItem>();

        var model = new CapabilityCatalogueItemsExpanderModel
        {
            CatalogueItems = expected,
        };

        model.CatalogueItems.Should().BeSameAs(expected);
    }
}
