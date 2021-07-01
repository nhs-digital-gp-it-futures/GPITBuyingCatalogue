using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Identity
{
    public static class IdentityServiceTests
    {
        [Fact]
        public static void Constructor_HttpAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new IdentityService(null));
        }

        [Fact]
        public static void Constructor_NoContext_ThrowsInvalidOperationException()
        {
            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(a => a.HttpContext).Returns((HttpContext)null);

            Assert.Throws<InvalidOperationException>(() => _ = new IdentityService(mockAccessor.Object).GetUserInfo());
        }

        [Fact]
        public static void GetUserInfo_GetsValue()
        {
            var testUserId = Guid.NewGuid();

            var identity = new ClaimsIdentity(new Claim[] 
            { 
                new(IdentityService.UserDisplayName, "Bill Smith"),
                new(IdentityService.UserId, testUserId.ToString())
            }, "mock");

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.User).Returns(new ClaimsPrincipal(identity));

            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

            var service = new IdentityService(mockAccessor.Object);

            (Guid userId, string userName) = service.GetUserInfo();

            Assert.Equal("Bill Smith", userName);
            Assert.Equal(testUserId, userId);
        }
    }
}
