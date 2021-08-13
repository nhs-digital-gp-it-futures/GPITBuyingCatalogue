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
    public sealed class ControllerBaseCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new Postprocessor(
                        new MethodInvoker(new ModestConstructorQuery()),
                        new ControllerBaseSpecimenCommand()),
                    new ControllerBaseRequestSpecification()));
        }

        private sealed class ControllerBaseSpecimenCommand : ISpecimenCommand
        {
            public void Execute(object specimen, ISpecimenContext context)
            {
                if (specimen is null)
                    throw new ArgumentNullException(nameof(specimen));

                if (context is null)
                    throw new ArgumentNullException(nameof(context));

                if (specimen is ControllerBase controller)
                {
                    var httpContextMock = context.Create<Mock<HttpContext>>();
                    httpContextMock
                        .Setup(c => c.User)
                        .Returns(CreateClaimsPrincipal(GetOrganisationId(context)));

                    controller.ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = CreateClaimsPrincipal(GetOrganisationId(context)),
                        },
                    };

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

        private sealed class ControllerBaseRequestSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request) =>
                request is Type type && typeof(ControllerBase).IsAssignableFrom(type);
        }
    }
}
