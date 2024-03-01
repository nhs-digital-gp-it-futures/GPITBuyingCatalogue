using System;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NSubstitute;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

internal sealed class IdentityServiceSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (!(request as Type == typeof(IIdentityService)))
            return new NoSpecimen();

        var userId = context.Create<int>();

        var identityServiceMock = context.Create<IIdentityService>();
        identityServiceMock.GetUserId().Returns(userId);

        return identityServiceMock;
    }
}
