using System;
using AutoFixture;
using AutoFixture.Kernel;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class MockIdentityServiceSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request as Type == typeof(IIdentityService)))
                return new NoSpecimen();

            var userId = context.Create<int>();
            var userName = context.Create<string>();

            var identityServiceMock = context.Create<Mock<IIdentityService>>();
            identityServiceMock.Setup(i => i.GetUserId()).Returns(userId);

            return identityServiceMock.Object;
        }
    }
}
