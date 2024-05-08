using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderDescription;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderDescriptionControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrderDescriptionController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(OrderDescriptionController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderDescriptionController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_OrderDescription_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            OrderDescriptionController controller)
        {
            var expectedViewData = new OrderDescriptionModel(internalOrgId, order) { BackLink = "testUrl" };

            orderServiceMock
                .GetOrderThin(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            var actualResult = await controller.OrderDescription(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OrderDescription_SetsDescription_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            OrderDescriptionModel model,
            [Frozen] IOrderDescriptionService orderDescriptionServiceMock,
            OrderDescriptionController controller)
        {
            var actualResult = await controller.OrderDescription(internalOrgId, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId } });
            await orderDescriptionServiceMock
                .Received()
                .SetOrderDescription(callOffId, internalOrgId, model.Description);
        }

        [Theory]
        [MockAutoData]
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
            actualResult.ViewData.ModelState.Keys.First().Should().Be(errorKey);
            actualResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(errorMessage);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_NewOrderDescription_ReturnsExpectedResult(
            string internalOrgId,
            OrderTypeEnum orderType,
            string frameworkId,
            string organisationName,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            OrderDescriptionController controller)
        {
            const int organisationId = 1;

            mockUsersService
                .GetUser(1)
                .Returns(new AspNetUser
                {
                    PrimaryOrganisationId = organisationId,
                });

            mockOrganisationsService
                .GetOrganisation(organisationId)
                .Returns(new Organisation
                {
                    Name = organisationName,
                });

            var expectedViewData = new OrderDescriptionModel(internalOrgId, organisationName) { BackLink = "testUrl" };

            var actualResult = await controller.NewOrderDescription(internalOrgId, orderType, frameworkId);

            await mockUsersService
                .Received()
                .GetUser(1);

            await mockOrganisationsService
                .Received()
                .GetOrganisation(organisationId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData(CatalogueItemType.AssociatedService)]
        [MockInlineAutoData(CatalogueItemType.Solution)]
        public static async Task Post_NewOrderDescription_AssociatedServicesOnly_AsExpected(
            OrderTypeEnum orderType,
            string frameworkId,
            OrderDescriptionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            OrderDescriptionController controller)
        {
            orderServiceMock.CreateOrder(model.Description, model.InternalOrgId, orderType, frameworkId).Returns(order);

            _ = await controller.NewOrderDescription(model.InternalOrgId, model, orderType, frameworkId);

            await orderServiceMock
                .Received()
                .CreateOrder(model.Description, model.InternalOrgId, orderType, frameworkId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_NewOrderDescription_CreatesOrder_CorrectlyRedirects(
            string internalOrgId,
            OrderTypeEnum orderType,
            string frameworkId,
            OrderDescriptionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            OrderDescriptionController controller)
        {
            orderServiceMock
                .CreateOrder(model.Description, model.InternalOrgId, Arg.Any<OrderTypeEnum>(), Arg.Any<string>())
                .Returns(order);

            var actualResult = await controller.NewOrderDescription(internalOrgId, model, orderType, frameworkId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_NewOrderDescription_InvalidModelState_ReturnsView(
            string internalOrgId,
            OrderTypeEnum orderType,
            string frameworkId,
            string errorKey,
            string errorMessage,
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            controller.ModelState.AddModelError(errorKey, errorMessage);

            var actualResult = (await controller.NewOrderDescription(internalOrgId, model, orderType, frameworkId)).As<ViewResult>();

            actualResult.Should().NotBeNull();
            actualResult.ViewName.Should().Be("OrderDescription");
            actualResult.ViewData.ModelState.IsValid.Should().BeFalse();
            actualResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            actualResult.ViewData.ModelState.Keys.First().Should().Be(errorKey);
            actualResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(errorMessage);
        }
    }
}
