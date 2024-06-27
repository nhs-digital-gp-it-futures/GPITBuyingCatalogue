using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Session;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.SharedMocks;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Session
{
    public static class SessionServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SessionService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_NullHttpContext_ThrowsException(
            IHttpContextAccessor httpContextAccessorMock)
        {
            httpContextAccessorMock.HttpContext.Returns((HttpContext)null);

            Assert.Throws<InvalidOperationException>(() => _ = new SessionService(httpContextAccessorMock));
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_NullSession_ThrowsException(
            HttpContext httpContextMock,
            IHttpContextAccessor httpContextAccessorMock)
        {
            httpContextMock.Session.Returns((ISession)null);
            httpContextAccessorMock.HttpContext.Returns(httpContextMock);

            Assert.Throws<InvalidOperationException>(() => _ = new SessionService(httpContextAccessorMock));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
        [MockAutoData]
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
            var mockHttpContext = Substitute.For<HttpContext>();
            mockHttpContext.Session.Returns(new MockHttpSession());

            var mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            mockHttpContextAccessor.HttpContext.Returns(mockHttpContext);

            return mockHttpContextAccessor;
        }
    }
}
