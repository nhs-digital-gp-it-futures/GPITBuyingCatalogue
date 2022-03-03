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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
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
            string internalOrgId,
            string selectionMode,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            var model = new SelectAdditionalServiceRecipientsModel(internalOrgId, state, selectionMode);

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            var actualResult = await controller.SelectAdditionalServiceRecipients(internalOrgId, state.CallOffId, selectionMode);

            orderSessionServiceMock.VerifyAll();

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServiceRecipients_StateDoesNotHaveRecipients_ReturnsExpectedResult(
            string internalOrgId,
            string selectionMode,
            [Frozen] IReadOnlyList<ServiceRecipient> recipients,
            CreateOrderItemModel state,
            [Frozen] Mock<IOdsService> odsServiceMock,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            state.ServiceRecipients = recipients
                .Select(r => new OrderItemRecipientModel(r))
                .ToList();

            var model = new SelectAdditionalServiceRecipientsModel(internalOrgId, state, selectionMode);

            state.ServiceRecipients = null;

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            odsServiceMock
                .Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(recipients);

            var actualResult = await controller.SelectAdditionalServiceRecipients(internalOrgId, state.CallOffId, selectionMode);

            orderSessionServiceMock.VerifyAll();
            odsServiceMock.VerifyAll();

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServiceRecipients_StateDoesNotHaveRecipients_SetsSelectedOnServiceRecipients(
            string internalOrgId,
            string selectionMode,
            [Frozen] IReadOnlyList<ServiceRecipient> recipients,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel state,
            OrderItem orderItem,
            [Frozen] Mock<IOdsService> odsServiceMock,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            state.CatalogueItemId = catalogueItemId;
            state.ServiceRecipients = recipients
                .Select(r => new OrderItemRecipientModel(r) { Selected = true })
                .ToList();

            var model = new SelectAdditionalServiceRecipientsModel(internalOrgId, state, selectionMode);

            state.ServiceRecipients = null;

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            odsServiceMock
                .Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(recipients);

            orderItem.CatalogueItemId = catalogueItemId;
            orderItem.SetRecipients(recipients.Select(x => new OrderItemRecipient { OdsCode = x.OrgId, }));

            orderItemServiceMock
                .Setup(x => x.GetOrderItems(state.CallOffId, internalOrgId, CatalogueItemType.Solution))
                .ReturnsAsync(new List<OrderItem> { orderItem });

            var actualResult = await controller.SelectAdditionalServiceRecipients(internalOrgId, state.CallOffId, selectionMode);

            orderSessionServiceMock.VerifyAll();
            orderItemServiceMock.VerifyAll();
            odsServiceMock.VerifyAll();

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAdditionalServiceRecipients_StateUpdatedCorrectly(
            string internalOrgId,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
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

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            await controller.SelectAdditionalServiceRecipients(internalOrgId, state.CallOffId, model);

            orderSessionServiceMock.VerifyAll();

            updatedState.ServiceRecipients.Should().BeEquivalentTo(serviceRecipients);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAdditionalServiceRecipients_NewSolution_CorrectlyRedirects(
            string internalOrgId,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = true;
            state.HasHitEditSolution = false;

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = await controller.SelectAdditionalServiceRecipients(internalOrgId, state.CallOffId, model);

            orderSessionServiceMock.VerifyAll();

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServiceRecipientsDateController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", state.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAdditionalServiceRecipients_ExistingSolution_CorrectlyRedirects(
            string internalOrgId,
            CreateOrderItemModel state,
            List<OrderItemRecipientModel> serviceRecipients,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsController controller)
        {
            serviceRecipients.First().Selected = true;
            state.IsNewSolution = false;
            state.HasHitEditSolution = true;

            orderSessionServiceMock
                .Setup(s => s.GetOrderStateFromSession(state.CallOffId))
                .Returns(state);

            var model = new SelectAdditionalServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients,
            };

            var actualResult = await controller.SelectAdditionalServiceRecipients(internalOrgId, state.CallOffId, model);

            orderSessionServiceMock.VerifyAll();

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServicesController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", state.CallOffId },
                { "CatalogueItemId", state.CatalogueItemId },
            });
        }
    }
}
