using System.Collections.Generic;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class AdditionalServiceRecipientsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AdditionalServiceRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AdditionalServiceRecipientsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServiceRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServiceRecipients_StateHasRecipients_ReturnsExpectedResult(
            string odsCode,
            string selectionMode,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            var expectedViewData = new SelectAdditionalServiceRecipientsModel(odsCode, state, selectionMode);
            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = await controller.SelectAdditionalServiceRecipients(odsCode, state.CallOffId, selectionMode);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServiceRecipients_StateDoesNotHaveRecipients_ReturnsExpectedResult(
            string odsCode,
            string selectionMode,
            [Frozen] IReadOnlyList<ServiceRecipient> recipients,
            CreateOrderItemModel state,
            [Frozen] Mock<IOdsService> odsServiceMock,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            state.ServiceRecipients = recipients.Select(r => new OrderItemRecipientModel(r)).ToList();
            var expectedViewData = new SelectAdditionalServiceRecipientsModel(odsCode, state, selectionMode);
            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            state.ServiceRecipients = null;
            odsServiceMock.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(odsCode)).ReturnsAsync(recipients);

            var actualResult = await controller.SelectAdditionalServiceRecipients(odsCode, state.CallOffId, selectionMode);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectAdditionalServiceRecipients_StateUpdatedCorrectly(
            string odsCode,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            CreateOrderItemModel updatedState = null;

            orderSessionServiceMock.Setup(s => s.SetOrderStateToSession(It.IsAny<CreateOrderItemModel>()))
                .Callback<CreateOrderItemModel>(s => updatedState = s);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            controller.SelectAdditionalServiceRecipients(odsCode, state.CallOffId, model);

            updatedState.ServiceRecipients.Should().BeEquivalentTo(serviceRecipients);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectAdditionalServiceRecipients_NewSolution_CorrectlyRedirects(
            string odsCode,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = true;
            state.HasHitEditSolution = false;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = controller.SelectAdditionalServiceRecipients(odsCode, state.CallOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServiceRecipientsDateController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", state.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectAdditionalServiceRecipients_ExistingSolution_CorrectlyRedirects(
            string odsCode,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = false;
            state.HasHitEditSolution = true;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = controller.SelectAdditionalServiceRecipients(odsCode, state.CallOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServicesController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", state.CallOffId }, { "CatalogueItemId", state.CatalogueItemId } });
        }
    }
}
