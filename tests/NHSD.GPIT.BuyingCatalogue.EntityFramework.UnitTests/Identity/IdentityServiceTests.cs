using System.Security.Claims;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Identity
{
    public static class IdentityServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(IdentityService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_NoContext_ReturnsNull(
            [Frozen] IHttpContextAccessor mockAccessor,
            IdentityService service)
        {
            mockAccessor.HttpContext.Returns((HttpContext)null);

            Assert.Null(service.GetUserId());
        }

        [Theory]
        [MockAutoData]
        public static void GetUserInfo_GetsValue(
            HttpContext mockContext,
            [Frozen] IHttpContextAccessor mockAccessor,
            IdentityService service)
        {
            const int testUserId = 67;

            var identity = new ClaimsIdentity(
                new Claim[]
                {
                    new(IdentityService.UserDisplayName, "Bill Smith"),
                    new(IdentityService.UserId, testUserId.ToString()),
                },
                "mock");

            mockContext.User.Returns(new ClaimsPrincipal(identity));
            mockAccessor.HttpContext.Returns(mockContext);

            var userId = service.GetUserId();

            userId.Should().Be(testUserId);
        }
    }
}
