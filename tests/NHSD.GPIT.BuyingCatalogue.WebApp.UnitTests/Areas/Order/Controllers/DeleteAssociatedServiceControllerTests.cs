using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteAssociatedService;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class DeleteAssociatedServiceControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DeleteAssociatedServiceController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(DeleteAssociatedServiceController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DeleteAssociatedServiceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteAssociatedService_ReturnsExpectedResult(
                string odsCode,
                EntityFramework.Ordering.Models.Order order,
                CatalogueItemId catalogueItemId,
                string catalogueItemName,
                [Frozen] Mock<IOrderService> orderServiceMock,
                DeleteAssociatedServiceController controller)
        {
            var expectedViewData = new DeleteAssociatedServiceModel(odsCode, order.CallOffId, catalogueItemId, catalogueItemName, order.Description);

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            var actualResult = await controller.DeleteAssociatedService(odsCode, order.CallOffId, catalogueItemId, catalogueItemName);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteAssociatedService_Deletes_CorrectlyRedirects(
            string odsCode,
            CallOffId callOffId,
            DeleteAssociatedServiceModel model,
            CatalogueItemId catalogueItemId,
            string catalogueItemName,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            DeleteAssociatedServiceController controller)
        {
            var actualResult = await controller.DeleteAssociatedService(odsCode, callOffId, catalogueItemId, catalogueItemName, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AssociatedServicesController.Index));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", callOffId } });
            orderItemServiceMock.Verify(o => o.DeleteOrderItem(callOffId, catalogueItemId), Times.Once);
        }
    }
}
