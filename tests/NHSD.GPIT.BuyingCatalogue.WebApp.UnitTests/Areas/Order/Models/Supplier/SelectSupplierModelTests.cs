using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class SelectSupplierModelTests
    {
        [Theory]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
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
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
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
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static void HasInsetText(OrderTypeEnum orderType)
        {
            var model = new SelectSupplierModel();
            model.OrderType = orderType;

            model.GetInsetText().Should().NotBeNullOrEmpty();
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        public static void DoesNotHaveInsetText(OrderTypeEnum orderType)
        {
            var model = new SelectSupplierModel();
            model.OrderType = orderType;

            model.GetInsetText().Should().BeNullOrEmpty();
        }
    }
}
