using System;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipientsDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class AdditionalServiceRecipientsDateControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AdditionalServiceRecipientsDateController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AdditionalServiceRecipientsDateController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServiceRecipientsDateController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServiceRecipientsDate_ReturnsExpectedResult(
            string internalOrgId,
            DateTime? defaultDeliveryDate,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsDateController controller)
        {
            var expectedViewData = new SelectAdditionalServiceRecipientsDateModel(state, defaultDeliveryDate);
            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = await controller.SelectAdditionalServiceRecipientsDate(internalOrgId, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAdditionalServiceRecipientsDate_Declarative_CorrectlyRedirects(
            string internalOrgId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsDateController controller)
        {
            var model = new SelectAdditionalServiceRecipientsDateModel
            {
                Day = DateTime.UtcNow.AddDays(1).Day.ToString(),
                Month = DateTime.UtcNow.AddDays(1).Month.ToString(),
                Year = DateTime.UtcNow.AddDays(1).Year.ToString(),
            };

            state.CataloguePrice.ProvisioningType = ProvisioningType.Declarative;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = await controller.SelectAdditionalServiceRecipientsDate(internalOrgId, state.CallOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.SelectFlatDeclarativeQuantity));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServicesController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", state.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAdditionalServiceRecipientsDate_OnDemand_CorrectlyRedirects(
            string internalOrgId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsDateController controller)
        {
            var model = new SelectAdditionalServiceRecipientsDateModel
            {
                Day = DateTime.UtcNow.AddDays(1).Day.ToString(),
                Month = DateTime.UtcNow.AddDays(1).Month.ToString(),
                Year = DateTime.UtcNow.AddDays(1).Year.ToString(),
            };

            state.CataloguePrice.ProvisioningType = ProvisioningType.OnDemand;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = await controller.SelectAdditionalServiceRecipientsDate(internalOrgId, state.CallOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.SelectFlatOnDemandQuantity));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServicesController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", state.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAdditionalServiceRecipientsDate_Patient_CorrectlyRedirects(
            string internalOrgId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AdditionalServiceRecipientsDateController controller)
        {
            var model = new SelectAdditionalServiceRecipientsDateModel
            {
                Day = DateTime.UtcNow.AddDays(1).Day.ToString(),
                Month = DateTime.UtcNow.AddDays(1).Month.ToString(),
                Year = DateTime.UtcNow.AddDays(1).Year.ToString(),
            };

            state.CataloguePrice.ProvisioningType = ProvisioningType.Patient;

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = await controller.SelectAdditionalServiceRecipientsDate(internalOrgId, state.CallOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(AdditionalServicesController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", state.CallOffId }, { "CatalogueItemId", state.CatalogueItemId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAdditionalServiceRecipientsDate_SetsDatesAndSession(
            string internalOrgId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<IDefaultDeliveryDateService> defaultDeliveryDateServiceMock,
            AdditionalServiceRecipientsDateController controller)
        {
            var model = new SelectAdditionalServiceRecipientsDateModel
            {
                Day = DateTime.UtcNow.AddDays(1).Day.ToString(),
                Month = DateTime.UtcNow.AddDays(1).Month.ToString(),
                Year = DateTime.UtcNow.AddDays(1).Year.ToString(),
            };

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            CreateOrderItemModel updatedState = null;

            orderSessionServiceMock.Setup(s => s.SetOrderStateToSession(It.IsAny<CreateOrderItemModel>()))
                .Callback<CreateOrderItemModel>(s => updatedState = s);

            await controller.SelectAdditionalServiceRecipientsDate(internalOrgId, state.CallOffId, model);

            updatedState.PlannedDeliveryDate.Should().Be(DateTime.UtcNow.AddDays(1).Date);

            defaultDeliveryDateServiceMock.Verify(c => c.SetDefaultDeliveryDate(state.CallOffId, internalOrgId, state.CatalogueItemId.GetValueOrDefault(), DateTime.UtcNow.AddDays(1).Date), Times.Once());
        }
    }
}
