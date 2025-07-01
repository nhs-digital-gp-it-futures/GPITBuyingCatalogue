using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class OrderSublocationCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderSublocation> composer)
            {
                return composer
                    .Without(x => x.Order)
                    .Without(x => x.OrderId);
            }

            fixture.Customize<OrderSublocation>(ComposerTransformation);
        }
    }
}
