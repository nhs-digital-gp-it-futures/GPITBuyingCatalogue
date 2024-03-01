using System;
using AutoFixture;
using AutoFixture.Kernel;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class MoqIdentityServiceSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request as Type == typeof(IIdentityService)))
                return new NoSpecimen();

            var userId = context.Create<int>();

            var identityServiceMock = context.Create<Mock<IIdentityService>>();
            identityServiceMock.Setup(i => i.GetUserId()).Returns(userId);

            return identityServiceMock.Object;
        }
    }
}
