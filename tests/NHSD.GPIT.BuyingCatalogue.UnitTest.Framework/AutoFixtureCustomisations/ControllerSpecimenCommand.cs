using System;
using System.Security.Claims;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NSubstitute;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

internal class ControllerSpecimenCommand : ISpecimenCommand
{
    public virtual void Execute(object specimen, ISpecimenContext context)
    {
        if (specimen is null)
            throw new ArgumentNullException(nameof(specimen));

        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (specimen is Controller controller)
        {
            var httpRequestCookiesMock = context.Create<IRequestCookieCollection>();
            var headerDictionary = context.Create<HeaderDictionary>();
            var httpRequestMock = context.Create<HttpRequest>();
            httpRequestMock.Headers.Returns(headerDictionary);
            httpRequestMock.Cookies.Returns(httpRequestCookiesMock);

            var httpResponseCookiesMock = context.Create<IResponseCookies>();
            var httpResponseMock = context.Create<HttpResponse>();
            httpResponseMock.Cookies.Returns(httpResponseCookiesMock);

            var featureCollectionMock = context.Create<IFeatureCollection>();

            var httpContextMock = context.Create<HttpContext>();
            httpContextMock.Request.Returns(httpRequestMock);
            httpContextMock.Response.Returns(httpResponseMock);
            httpContextMock.Features.Returns(featureCollectionMock);
            httpContextMock.User
                .Returns(CreateClaimsPrincipal(GetOrganisationId(context)));

            controller.ControllerContext = new ControllerContext { HttpContext = httpContextMock };

            var urlHelperMock = context.Create<IUrlHelper>();
            urlHelperMock.Action(Arg.Any<UrlActionContext>())
                .Returns("testUrl");

            var tempDataMock = context.Create<ITempDataDictionary>();
            controller.TempData = tempDataMock;

            controller.Url = urlHelperMock;
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
