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
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class CatalogueSolutionRecipientsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CatalogueSolutionRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(CatalogueSolutionRecipientsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CatalogueSolutionRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolutionServiceRecipients_StateHasRecipients_ReturnsExpectedResult(
                string odsCode,
                string selectionMode,
                CreateOrderItemModel state,
                [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
                CatalogueSolutionRecipientsController controller)
        {
            var expectedViewData = new SelectSolutionServiceRecipientsModel(odsCode, state, selectionMode);
            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = await controller.SelectSolutionServiceRecipients(odsCode, state.CallOffId, selectionMode);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolutionRecipients_StateDoesNotHaveRecipients_ReturnsExpectedResult(
            string odsCode,
            string selectionMode,
            [Frozen] IReadOnlyList<ServiceContracts.Models.ServiceRecipient> recipients,
            CreateOrderItemModel state,
            [Frozen] Mock<IOdsService> odsServiceMock,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionRecipientsController controller)
        {
            state.ServiceRecipients = recipients.Select(r => new OrderItemRecipientModel(r)).ToList();
            var expectedViewData = new SelectSolutionServiceRecipientsModel(odsCode, state, selectionMode);
            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            state.ServiceRecipients = null;
            odsServiceMock.Setup(s => s.GetServiceRecipientsByParentOdsCode(odsCode)).ReturnsAsync(recipients);

            var actualResult = await controller.SelectSolutionServiceRecipients(odsCode, state.CallOffId, selectionMode);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectSolutionRecipients_NoRecipientsSelected_ReturnsErrorResult(
            string odsCode,
            CallOffId callOffId,
            List<OrderItemRecipientModel> serviceRecipients,
            CatalogueSolutionRecipientsController controller)
        {
            serviceRecipients.ForEach(r => r.Selected = false);

            var model = new SelectSolutionServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = controller.SelectSolutionServiceRecipients(odsCode, callOffId, model);

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
        public static void Post_SelectSolutionRecipients_StateUpdatedCorrectly(
            string odsCode,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            CreateOrderItemModel updatedState = null;

            orderSessionServiceMock.Setup(s => s.SetOrderStateToSession(It.IsAny<CreateOrderItemModel>()))
                .Callback<CreateOrderItemModel>(s => updatedState = s);

            var model = new SelectSolutionServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            controller.SelectSolutionServiceRecipients(odsCode, state.CallOffId, model);

            updatedState.ServiceRecipients.Should().BeEquivalentTo(serviceRecipients);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectSolutionRecipients_NewSolution_CorrectlyRedirects(
            string odsCode,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = true;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var model = new SelectSolutionServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = controller.SelectSolutionServiceRecipients(odsCode, state.CallOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(CatalogueSolutionRecipientsDateController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", state.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectSolutionRecipients_ExistingSolution_CorrectlyRedirects(
            string odsCode,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = false;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var model = new SelectSolutionServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = controller.SelectSolutionServiceRecipients(odsCode, state.CallOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CatalogueSolutionsController.EditSolution));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", state.CallOffId }, { "CatalogueItemId", state.CatalogueItemId } });
        }
    }
}
