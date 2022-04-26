using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class ControllerCustomization : ICustomization
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

        private sealed class ControllerSpecimenCommand : ControllerBaseSpecimenCommand
        {
            public override void Execute(object specimen, ISpecimenContext context)
            {
                base.Execute(specimen, context);

                if (specimen is Controller controller)
                {
                    var tempDataMock = context.Create<Mock<ITempDataDictionary>>();
                    controller.TempData = tempDataMock.Object;
                }
                else
                {
                    throw new ArgumentException("The specimen must be an instance of Controller", nameof(specimen));
                }
            }
        }

        private sealed class ControllerRequestSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request) =>
                request is Type type && typeof(Controller).IsAssignableFrom(type);
        }
    }
}
