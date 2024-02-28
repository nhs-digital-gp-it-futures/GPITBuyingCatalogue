using System;
using System.Security.Claims;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal class MoqControllerSpecimenCommand : ISpecimenCommand
    {
        public virtual void Execute(object specimen, ISpecimenContext context)
        {
            if (specimen is null)
                throw new ArgumentNullException(nameof(specimen));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (specimen is Controller controller)
            {
                var httpRequestCookiesMock = context.Create<Mock<IRequestCookieCollection>>();
                var headerDictionary = context.Create<HeaderDictionary>();
                var httpRequestMock = context.Create<Mock<HttpRequest>>();
                httpRequestMock.Setup(r => r.Headers).Returns(headerDictionary);
                httpRequestMock.Setup(r => r.Cookies).Returns(httpRequestCookiesMock.Object);

                var httpResponseCookiesMock = context.Create<Mock<IResponseCookies>>();
                var httpResponseMock = context.Create<Mock<HttpResponse>>();
                httpResponseMock.Setup(r => r.Cookies).Returns(httpResponseCookiesMock.Object);

                var featureCollectionMock = context.Create<Mock<IFeatureCollection>>();

                var httpContextMock = context.Create<Mock<HttpContext>>();
                httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);
                httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);
                httpContextMock.Setup(c => c.Features).Returns(featureCollectionMock.Object);
                httpContextMock
                    .Setup(c => c.User)
                    .Returns(CreateClaimsPrincipal(GetOrganisationId(context)));

                controller.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

                var urlHelperMock = context.Create<Mock<IUrlHelper>>();
                urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                    .Returns("testUrl");

                var tempDataMock = context.Create<Mock<ITempDataDictionary>>();
                controller.TempData = tempDataMock.Object;

                controller.Url = urlHelperMock.Object;
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
                new Claim(CatalogueClaims.UserId, 1.ToString()),
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
