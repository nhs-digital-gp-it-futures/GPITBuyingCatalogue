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
                string internalOrgId,
                string selectionMode,
                CreateOrderItemModel state,
                [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
                CatalogueSolutionRecipientsController controller)
        {
            var model = new SelectSolutionServiceRecipientsModel(internalOrgId, state, selectionMode);

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            var actualResult = await controller.SelectSolutionServiceRecipients(internalOrgId, state.CallOffId, selectionMode);

            orderSessionServiceMock.VerifyAll();

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolutionRecipients_StateDoesNotHaveRecipients_ReturnsExpectedResult(
            string internalOrgId,
            string selectionMode,
            [Frozen] IReadOnlyList<ServiceRecipient> recipients,
            CreateOrderItemModel state,
            [Frozen] Mock<IOdsService> odsServiceMock,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionRecipientsController controller)
        {
            state.ServiceRecipients = recipients
                .Select(r => new OrderItemRecipientModel(r))
                .ToList();

            var model = new SelectSolutionServiceRecipientsModel(internalOrgId, state, selectionMode);

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            state.ServiceRecipients = null;

            odsServiceMock
                .Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(recipients);

            var actualResult = await controller.SelectSolutionServiceRecipients(internalOrgId, state.CallOffId, selectionMode);

            orderSessionServiceMock.VerifyAll();
            odsServiceMock.VerifyAll();

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectSolutionRecipients_StateUpdatedCorrectly(
            string odsCode,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            CreateOrderItemModel updatedState = null;

            orderSessionServiceMock
                .Setup(s => s.SetOrderStateToSession(It.IsAny<CreateOrderItemModel>()))
                .Callback<CreateOrderItemModel>(s => updatedState = s)
                .Verifiable();

            var model = new SelectSolutionServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            await controller.SelectSolutionServiceRecipients(odsCode, state.CallOffId, model);

            orderSessionServiceMock.VerifyAll();

            updatedState.ServiceRecipients.Should().BeEquivalentTo(serviceRecipients);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectSolutionRecipients_NewSolution_CorrectlyRedirects(
            string internalOrgId,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = true;
            state.HasHitEditSolution = false;

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            var model = new SelectSolutionServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = await controller.SelectSolutionServiceRecipients(internalOrgId, state.CallOffId, model);

            orderSessionServiceMock.VerifyAll();

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(CatalogueSolutionRecipientsDateController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", state.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectSolutionRecipients_ExistingSolution_CorrectlyRedirects(
            string internalOrgId,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = false;
            state.HasHitEditSolution = true;

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            var model = new SelectSolutionServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = await controller.SelectSolutionServiceRecipients(internalOrgId, state.CallOffId, model);

            orderSessionServiceMock.VerifyAll();

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CatalogueSolutionsController.EditSolution));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", state.CallOffId },
                { "CatalogueItemId", state.CatalogueItemId },
            });
        }
    }
}
