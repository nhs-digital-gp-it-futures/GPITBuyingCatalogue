using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class SelectSupplierModelTests
    {
        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static void Uses_SupplierSearch(
            OrderTypeEnum orderType,
            CallOffId callOffId)
        {
            var model = new SelectSupplierModel();
            model.CallOffId = callOffId;
            model.OrderType = orderType;

            model.GetPageTitle().Should().Be(SelectSupplierModel.StandardPageTitle with { Caption = $"Order {callOffId}" });
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static void Uses_SupplierSelection(
            OrderTypeEnum orderType,
            CallOffId callOffId)
        {
            var model = new SelectSupplierModel();
            model.CallOffId = callOffId;
            model.OrderType = orderType;

            model.GetPageTitle().Should().Be(SelectSupplierModel.SelectionPageTitle with { Caption = $"Order {callOffId}" });
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static void HasInsetText(OrderTypeEnum orderType)
        {
            var model = new SelectSupplierModel();
            model.OrderType = orderType;

            model.GetInsetText().Should().NotBeNullOrEmpty();
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        public static void DoesNotHaveInsetText(OrderTypeEnum orderType)
        {
            var model = new SelectSupplierModel();
            model.OrderType = orderType;

            model.GetInsetText().Should().BeNullOrEmpty();
        }
    }
}
