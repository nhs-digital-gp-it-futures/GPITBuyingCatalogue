using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class ControllerCustomization(MockingFramework mockingFramework) : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenCommand controllerSpecimen = mockingFramework == MockingFramework.Moq
                ? new MoqControllerSpecimenCommand()
                : new ControllerSpecimenCommand();

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new Postprocessor(
                        new MethodInvoker(new ModestConstructorQuery()),
                        controllerSpecimen),
                    new ControllerRequestSpecification()));
        }

        private sealed class ControllerRequestSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request) =>
                request is Type type && typeof(Controller).IsAssignableFrom(type);
        }
    }
}
