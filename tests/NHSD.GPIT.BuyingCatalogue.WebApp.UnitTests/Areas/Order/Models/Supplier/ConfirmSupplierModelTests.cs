using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class ConfirmSupplierModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Uses_ConfirmTitle(CallOffId callOffId)
        {
            var model = new ConfirmSupplierModel();
            model.CallOffId = callOffId;
            model.OnlyOption = false;

            model.GetPageTitle().Should().Be(ConfirmSupplierModel.StandardSupplierConfirmationPageTitle with { Caption = $"Order {callOffId}" });
        }

        [Theory]
        [CommonAutoData]
        public static void Uses_SingleSupplierConfirmationPageTitle_ForMergersAndSplits(CallOffId callOffId)
        {
            var model = new ConfirmSupplierModel();
            model.CallOffId = callOffId;
            model.OnlyOption = true;

            model.GetPageTitle().Should().Be(ConfirmSupplierModel.SingleSupplierConfirmationPageTitle with { Caption = $"Order {callOffId}" });
        }
    }
}
