using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal class RecipientDateModelCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<RecipientDateModel> composer) => composer
                .FromFactory(new RecipientDateModelSpecimenBuilder())
                .Without(x => x.Day)
                .Without(x => x.Month)
                .Without(x => x.Year);

            fixture.Customize<RecipientDateModel>(ComposerTransformation);
        }

        private class RecipientDateModelSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(RecipientDateModel)))
                {
                    return new NoSpecimen();
                }

                var output = new RecipientDateModel();
                var date = context.Create<DateTime>();

                output.Day = $"{date.Day}";
                output.Month = $"{date.Month}";
                output.Year = $"{date.Year}";

                return output;
            }
        }
    }
}
