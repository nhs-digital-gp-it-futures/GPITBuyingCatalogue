using System;
using System.Security.Claims;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
        [CommonAutoData]
        public static void Constructor_NoContext_ThrowsInvalidOperationException(
            [Frozen] Mock<IHttpContextAccessor> mockAccessor,
            IdentityService service)
        {
            mockAccessor.Setup(a => a.HttpContext).Returns((HttpContext)null);

            Assert.Throws<InvalidOperationException>(() => _ = service.GetUserId());
        }

        [Theory]
        [CommonAutoData]
        public static void GetUserInfo_GetsValue(
            Mock<HttpContext> mockContext,
            [Frozen] Mock<IHttpContextAccessor> mockAccessor,
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

            mockContext.Setup(c => c.User).Returns(new ClaimsPrincipal(identity));
            mockAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

            var userId = service.GetUserId();

            userId.Should().Be(testUserId);
        }
    }
}
