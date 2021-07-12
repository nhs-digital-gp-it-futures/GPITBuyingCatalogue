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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            CallOffId callOffId,
            string selectionMode,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            var expectedViewData = new SelectAdditionalServiceRecipientsModel(odsCode, state, selectionMode);
            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(callOffId)).Returns(state);

            var actualResult = await controller.SelectAdditionalServiceRecipients(odsCode, callOffId, selectionMode);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServiceRecipients_StateDoesNotHaveRecipients_ReturnsExpectedResult(
            string odsCode,
            CallOffId callOffId,
            string selectionMode,
            [Frozen] IReadOnlyList<ServiceRecipient> recipients,
            CreateOrderItemModel state,
            [Frozen] Mock<IOdsService> odsServiceMock,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            state.ServiceRecipients = recipients.Select(r => new OrderItemRecipientModel(r)).ToList();
            var expectedViewData = new SelectAdditionalServiceRecipientsModel(odsCode, state, selectionMode);
            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(callOffId)).Returns(state);

            state.ServiceRecipients = null;
            odsServiceMock.Setup(s => s.GetServiceRecipientsByParentOdsCode(odsCode)).ReturnsAsync(recipients);

            var actualResult = await controller.SelectAdditionalServiceRecipients(odsCode, callOffId, selectionMode);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectAdditionalServiceRecipients_NoRecipientsSelected_ReturnsErrorResult(
            string odsCode,
            CallOffId callOffId,
            AdditionalServiceRecipientsController controller,
            List<OrderItemRecipientModel> serviceRecipients)
        {
            serviceRecipients.ForEach(r => r.Selected = false);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = controller.SelectAdditionalServiceRecipients(odsCode, callOffId, model);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.ModelState.ValidationState.Should().Be(ModelValidationState.Invalid);

            actualResult.As<ViewResult>()
                .ViewData.ModelState.Keys.Single()
                .Should()
                .Be("ServiceRecipients[0].Selected");

            actualResult.As<ViewResult>()
                .ViewData.ModelState.Values.Single()
                .Errors.Single()
                .ErrorMessage.Should()
                .Be("Select a Service Recipient");
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectAdditionalServiceRecipients_StateUpdatedCorrectly(
            string odsCode,
            CallOffId callOffId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller,
            List<OrderItemRecipientModel> serviceRecipients)
        {
            serviceRecipients.First().Selected = true;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(callOffId)).Returns(state);

            CreateOrderItemModel updatedState = null;

            orderSessionServiceMock.Setup(s => s.SetOrderStateToSession(It.IsAny<CreateOrderItemModel>()))
                .Callback<CreateOrderItemModel>(x => updatedState = x);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            controller.SelectAdditionalServiceRecipients(odsCode, callOffId, model);

            updatedState.ServiceRecipients.Should().BeEquivalentTo(serviceRecipients);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectAdditionalServiceRecipients_NewSolution_CorrectlyRedirects(
            string odsCode,
            CallOffId callOffId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller,
            List<OrderItemRecipientModel> serviceRecipients)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = true;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(callOffId)).Returns(state);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = controller.SelectAdditionalServiceRecipients(odsCode, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();

            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServiceRecipientsDateController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectAdditionalServiceRecipients_ExistingSolution_CorrectlyRedirects(
            string odsCode,
            CallOffId callOffId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller,
            List<OrderItemRecipientModel> serviceRecipients)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = false;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(callOffId)).Returns(state);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = controller.SelectAdditionalServiceRecipients(odsCode, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();

            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServicesController).ControllerName());
        }
    }
}
