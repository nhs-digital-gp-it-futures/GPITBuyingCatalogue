using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class ConfirmSupplierModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId, CallOffId callOffId, EntityFramework.Catalogue.Models.Supplier supplier)
        {
            var model = new ConfirmSupplierModel(internalOrgId, callOffId, supplier);

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.SupplierId.Should().Be(supplier.Id);
            model.Name.Should().Be(supplier.Name);
            model.LegalName.Should().Be(supplier.LegalName);
            model.Address.Should().Be(supplier.Address);
            model.Options.Should()
                .BeEquivalentTo(
                    new List<SelectOption<bool>>
                    {
                        new(ConfirmSupplierModel.YesOption, true), new(ConfirmSupplierModel.NoOption, false),
                    });
        }

        [Fact]
        public static void Uses_ConfirmTitle()
        {
            var model = new ConfirmSupplierModel { OnlyOption = false };

            model.GetPageTitle().Should().Be(ConfirmSupplierModel.StandardSupplierConfirmationPageTitle with { Caption = model.Name });
        }

        [Fact]
        public static void Uses_SingleSupplierConfirmationPageTitle_ForMergersAndSplits()
        {
            var model = new ConfirmSupplierModel { OnlyOption = true };

            model.GetPageTitle().Should().Be(ConfirmSupplierModel.SingleSupplierConfirmationPageTitle with { Caption = model.Name });
        }
    }
}
