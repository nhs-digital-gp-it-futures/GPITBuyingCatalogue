using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.FundingSource
{
    public static class FundingSourceControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FundingSourceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FundingSources_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            orderServiceMock.Setup(o => o.GetOrderWithOrderItemsForFunding(order.CallOffId, internalOrgId))
                .ReturnsAsync(order);

            var expectedViewData = new FundingSources(internalOrgId, order.CallOffId, order);

            var actual = await controller.FundingSources(internalOrgId, order.CallOffId);

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FundingSource_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            FundingSourceController controller)
        {
            orderItemServiceMock.Setup(oi => oi.GetOrderItem(order.CallOffId, internalOrgId, orderItem.CatalogueItemId))
                .ReturnsAsync(orderItem);

            var expectedViewData = new WebApp.Areas.Order.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, orderItem);

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId);

            orderItemServiceMock.VerifyAll();

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FundingSource_ReturnsExpectedRedirect(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            FundingSourceController controller)
        {
            var model = new WebApp.Areas.Order.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, orderItem)
            {
                SelectedFundingType = OrderItemFundingType.CentralFunding,
                AmountOfCentralFunding = orderItem.CalculateTotalCost(),
            };

            orderItemServiceMock.Setup(oi => oi.SaveOrUpdateOrderItemFunding(order.CallOffId, internalOrgId, orderItem.CatalogueItemId, model.SelectedFundingType, model.AmountOfCentralFunding))
                .ReturnsAsync(orderItem);

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId, model);

            orderItemServiceMock.VerifyAll();

            var actualResult = actual.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(FundingSourceController.FundingSources));
            actualResult.ControllerName.Should().Be(typeof(FundingSourceController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FundingSource_ModelError_ReturnsView(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            FundingSourceController controller)
        {
            var model = new WebApp.Areas.Order.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, orderItem);

            controller.ModelState.AddModelError("test", "test");

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId, model);

            orderItemServiceMock.VerifyAll();

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.ModelState.ValidationState.Should().Be(ModelValidationState.Invalid);
        }
    }
}
