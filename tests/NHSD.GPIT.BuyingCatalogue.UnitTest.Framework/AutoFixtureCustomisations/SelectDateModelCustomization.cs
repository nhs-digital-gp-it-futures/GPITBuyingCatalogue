using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal class SelectDateModelCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<SelectDateModel> composer) => composer
                .FromFactory(new SelectDateModelSpecimenBuilder())
                .Without(x => x.Day)
                .Without(x => x.Month)
                .Without(x => x.Year);

            fixture.Customize<SelectDateModel>(ComposerTransformation);
        }

        private class SelectDateModelSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(SelectDateModel)))
                {
                    return new NoSpecimen();
                }

                var output = new SelectDateModel();
                var date = context.Create<DateTime>();

                output.Day = $"{date.Day}";
                output.Month = $"{date.Month}";
                output.Year = $"{date.Year}";

                return output;
            }
        }
    }
}
