using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class DefaultDeliveryDateCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<DefaultDeliveryDate> composer) =>
                composer.Without(d => d.Order);

            fixture.Customize<DefaultDeliveryDate>(ComposerTransformation);
        }
    }
}
