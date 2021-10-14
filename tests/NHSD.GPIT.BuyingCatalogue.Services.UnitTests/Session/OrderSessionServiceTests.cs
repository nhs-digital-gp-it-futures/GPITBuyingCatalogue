using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.Session;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Session
{
    public static class OrderSessionServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderSessionService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void CreateOrderItemModel_StoredAndRetrieved_FromSession(
            Mock<IOrderItemService> mockOrderItemService,
            Mock<IOrderService> mockOrderService,
            Mock<ISolutionsService> mockSolutionsService,
            Mock<IOdsService> mockOdsService,
            Mock<IOrganisationsService> mockOrganisationService,
            CreateOrderItemModel model)
        {
            var service = new OrderSessionService(
                new SessionService(GetAccessor()),
                mockOrderItemService.Object,
                mockOrderService.Object,
                mockSolutionsService.Object,
                mockOdsService.Object,
                mockOrganisationService.Object);

            service.SetOrderStateToSession(model);

            var actual = service.GetOrderStateFromSession(model.CallOffId);

            actual.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void ClearSession_Removes_FromSession(
           Mock<IOrderItemService> mockOrderItemService,
           Mock<IOrderService> mockOrderService,
           Mock<ISolutionsService> mockSolutionsService,
           Mock<IOdsService> mockOdsService,
           Mock<IOrganisationsService> mockOrganisationService,
           CreateOrderItemModel model)
        {
            var service = new OrderSessionService(
                new SessionService(GetAccessor()),
                mockOrderItemService.Object,
                mockOrderService.Object,
                mockSolutionsService.Object,
                mockOdsService.Object,
                mockOrganisationService.Object);

            service.SetOrderStateToSession(model);

            service.ClearSession(model.CallOffId);

            var actual = service.GetOrderStateFromSession(model.CallOffId);

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void SetPrice_UpdatesPrice_InSession(
           Mock<IOrderItemService> mockOrderItemService,
           Mock<IOrderService> mockOrderService,
           Mock<ISolutionsService> mockSolutionsService,
           Mock<IOdsService> mockOdsService,
           Mock<IOrganisationsService> mockOrganisationService,
           CreateOrderItemModel model,
           CataloguePrice price)
        {
            price.CurrencyCode = "GBP";

            var service = new OrderSessionService(
                new SessionService(GetAccessor()),
                mockOrderItemService.Object,
                mockOrderService.Object,
                mockSolutionsService.Object,
                mockOdsService.Object,
                mockOrganisationService.Object);

            service.SetOrderStateToSession(model);

            var actual = service.SetPrice(model.CallOffId, price);

            actual.AgreedPrice.Should().Be(price.Price);
            actual.CataloguePrice.Should().BeEquivalentTo(price);
            actual.CurrencySymbol.Should().Be(CurrencyCodeSigns.Code[price.CurrencyCode]);
        }

        [Theory]
        [CommonAutoData]
        public static void SetOrderStateToSession_NullModel_ThrowsException(OrderSessionService service)
        {
            Assert.Throws<ArgumentNullException>(() => service.SetOrderStateToSession(null));
        }

        [Theory]
        [CommonAutoData]
        public static void SetPrice_NullCataloguePrice_ThrowsException(OrderSessionService service)
        {
            Assert.Throws<ArgumentNullException>(() => service.SetPrice(default, null));
        }

        private static IHttpContextAccessor GetAccessor()
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Session).Returns(new MockHttpSession());

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(ca => ca.HttpContext).Returns(mockHttpContext.Object);

            return mockHttpContextAccessor.Object;
        }
    }
}
