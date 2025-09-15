using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierServices;

public static class AssociatedServicesModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Supplier supplier,
        List<AssociatedService> associatedServices)
    {
        var model = new AssociatedServicesModel(supplier, associatedServices);

        model.SupplierId.Should().Be(supplier.Id);
        model.SupplierName.Should().Be(supplier.Name);
        model.AssociatedServices.Should().BeEquivalentTo(associatedServices);
    }
}
