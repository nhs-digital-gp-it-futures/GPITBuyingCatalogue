using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Session;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Session
{
    public static class SessionServiceTests
    {
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
            CatalogueItem solution)
        {
            var service = new SessionService(GetAccessor());

            service.SetObject(key, solution);

            var actual = service.GetObject<CatalogueItem>(key);
            actual.Should().BeEquivalentTo(solution);
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
