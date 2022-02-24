using System.Linq;
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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderDescriptionControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrderDescriptionController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(OrderDescriptionController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderDescriptionController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OrderDescription_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderDescriptionController controller)
        {
            var expectedViewData = new OrderDescriptionModel(internalOrgId, order) { BackLink = "testUrl" };

            orderServiceMock.Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            var actualResult = await controller.OrderDescription(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OrderDescription_SetsDescription_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            OrderDescriptionModel model,
            [Frozen] Mock<IOrderDescriptionService> orderDescriptionServiceMock,
            OrderDescriptionController controller)
        {
            var actualResult = await controller.OrderDescription(internalOrgId, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId } });
            orderDescriptionServiceMock.Verify(o => o.SetOrderDescription(callOffId, internalOrgId, model.Description), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OrderDescription_InvalidModelState_ReturnsView(
            string internalOrgId,
            CallOffId callOffId,
            string errorKey,
            string errorMessage,
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            controller.ModelState.AddModelError(errorKey, errorMessage);

            var actualResult = (await controller.OrderDescription(internalOrgId, callOffId, model)).As<ViewResult>();

            actualResult.Should().NotBeNull();
            actualResult.ViewName.Should().BeNull();
            actualResult.ViewData.ModelState.IsValid.Should().BeFalse();
            actualResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            actualResult.ViewData.ModelState.Keys.Single().Should().Be(errorKey);
            actualResult.ViewData.ModelState.Values.Single().Errors.Single().ErrorMessage.Should().Be(errorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NewOrderDescription_ReturnsExpectedResult(
            string internalOrgId,
            string organisationName,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            OrderDescriptionController controller)
        {
            const int organisationId = 1;

            mockUsersService
                .Setup(x => x.GetUser(1))
                .ReturnsAsync(new AspNetUser
                {
                    PrimaryOrganisationId = organisationId,
                });

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisationId))
                .ReturnsAsync(new Organisation
                {
                    Name = organisationName,
                });

            var expectedViewData = new OrderDescriptionModel(internalOrgId, organisationName) { BackLink = "testUrl" };

            var actualResult = await controller.NewOrderDescription(internalOrgId);

            mockUsersService.VerifyAll();
            mockOrganisationsService.VerifyAll();

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_NewOrderDescription_CreatesOrder_CorrectlyRedirects(
            string internalOrgId,
            OrderDescriptionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderDescriptionController controller)
        {
            orderServiceMock.Setup(s => s.CreateOrder(model.Description, model.InternalOrgId)).ReturnsAsync(order);

            var actualResult = await controller.NewOrderDescription(internalOrgId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId } });
            orderServiceMock.Verify(o => o.CreateOrder(model.Description, model.InternalOrgId), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_NewOrderDescription_InvalidModelState_ReturnsView(
            string internalOrgId,
            string errorKey,
            string errorMessage,
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            controller.ModelState.AddModelError(errorKey, errorMessage);

            var actualResult = (await controller.NewOrderDescription(internalOrgId, model)).As<ViewResult>();

            actualResult.Should().NotBeNull();
            actualResult.ViewName.Should().Be("OrderDescription");
            actualResult.ViewData.ModelState.IsValid.Should().BeFalse();
            actualResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            actualResult.ViewData.ModelState.Keys.Single().Should().Be(errorKey);
            actualResult.ViewData.ModelState.Values.Single().Errors.Single().ErrorMessage.Should().Be(errorMessage);
        }
    }
}
