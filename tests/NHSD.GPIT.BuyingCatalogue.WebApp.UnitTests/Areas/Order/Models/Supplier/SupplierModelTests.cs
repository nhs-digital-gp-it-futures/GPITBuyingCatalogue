using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class SupplierModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode, 
            EntityFramework.Ordering.Models.Order order, 
            ICollection<SupplierContact> supplierContacts)
        {
            var model = new SupplierModel(odsCode, order, supplierContacts);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{order.CallOffId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Supplier information for {order.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.Id.Should().Be(order.Supplier.Id);
            model.Name.Should().Be(order.Supplier.Name);
            model.Address.Should().BeEquivalentTo(order.Supplier.Address);

            // TODO: PrimaryContact
        }
    }
}
