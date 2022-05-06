using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Session;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.SharedMocks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Session
{
    public static class SessionServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SessionService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_NullHttpContext_ThrowsException(
            Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext)null);

            Assert.Throws<InvalidOperationException>(() => _ = new SessionService(httpContextAccessorMock.Object));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_NullSession_ThrowsException(
            Mock<HttpContext> httpContextMock,
            Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            httpContextMock.Setup(c => c.Session).Returns((ISession)null);
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

            Assert.Throws<InvalidOperationException>(() => _ = new SessionService(httpContextAccessorMock.Object));
        }

        [Theory]
        [CommonAutoData]
        public static void Strings_StoredAndRetrieved_FromSession(
            string key,
            string expected)
        {
            var service = new SessionService(GetAccessor());

            service.SetString(key, expected);
            var actual = service.GetString(key);

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Clear_Removes_FromSession(
            string key,
            string expected)
        {
            var service = new SessionService(GetAccessor());

            service.SetString(key, expected);
            service.ClearSession(key);
            var actual = service.GetString(key);
            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Objects_StoredAndRetrieved_FromSession(
            string key,
            CatalogueItem catalogueItem)
        {
            // Clear circular references
            catalogueItem.CatalogueItemCapabilities.Clear();
            catalogueItem.CataloguePrices.Clear();
            catalogueItem.Supplier.CatalogueItems.Clear();
            catalogueItem.CatalogueItemContacts.Clear();

            var service = new SessionService(GetAccessor());

            service.SetObject(key, catalogueItem);

            var actual = service.GetObject<CatalogueItem>(key);
            actual.Should().BeEquivalentTo(catalogueItem);
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
