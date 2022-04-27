using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
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

        private sealed class ControllerBaseRequestSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request) =>
                request is Type type && typeof(ControllerBase).IsAssignableFrom(type);
        }
    }
}
