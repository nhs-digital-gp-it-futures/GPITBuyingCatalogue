using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

internal sealed class ControllerCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(
            new FilteringSpecimenBuilder(
                new Postprocessor(
                    new MethodInvoker(new ModestConstructorQuery()),
                    new ControllerSpecimenCommand()),
                new ControllerRequestSpecification()));
    }

    private sealed class ControllerRequestSpecification : IRequestSpecification
    {
        public bool IsSatisfiedBy(object request) =>
            request is Type type && typeof(Controller).IsAssignableFrom(type);
    }
}
