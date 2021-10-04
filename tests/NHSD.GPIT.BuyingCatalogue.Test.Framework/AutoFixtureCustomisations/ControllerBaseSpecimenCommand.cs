using System;
using System.Security.Claims;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal class ControllerBaseSpecimenCommand : ISpecimenCommand
    {
        public virtual void Execute(object specimen, ISpecimenContext context)
        {
            if (specimen is null)
                throw new ArgumentNullException(nameof(specimen));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (specimen is ControllerBase controller)
            {
                var headerDictionary = context.Create<HeaderDictionary>();
                var httpRequestMock = context.Create<Mock<HttpRequest>>();
                httpRequestMock.Setup(r => r.Headers).Returns(headerDictionary);

                var httpResponseCookiesMock = context.Create<Mock<IResponseCookies>>();
                var httpResponseMock = context.Create<Mock<HttpResponse>>();
                httpResponseMock.Setup(r => r.Cookies).Returns(httpResponseCookiesMock.Object);

                var httpContextMock = context.Create<Mock<HttpContext>>();
                httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);
                httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);
                httpContextMock
                    .Setup(c => c.User)
                    .Returns(CreateClaimsPrincipal(GetOrganisationId(context)));

                controller.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

                // TODO: Investigate better way of doing this to test Url.Action in controllers
                var urlHelperMock = new Mock<IUrlHelper>();
                controller.Url = urlHelperMock.Object;
                urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                    .Returns("testUrl");
            }
            else
            {
                throw new ArgumentException("The specimen must be an instance of ControllerBase", nameof(specimen));
            }
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(int organisationId)
        {
            var claims = new[]
            {
                new Claim("primaryOrganisationId", organisationId.ToString()),
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        }

        private static int GetOrganisationId(ISpecimenContext context)
        {
            // Order must be frozen for this to work correctly
            var order = context.Create<Order>();
            return order.OrderingParty.Id;
        }
    }
}
