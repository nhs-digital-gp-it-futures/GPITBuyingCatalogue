using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class EpicCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Epic> composer) => composer
                .Without(e => e.Capabilities)
                .Without(e => e.CapabilityId)
                .Without(e => e.LastUpdatedByUser);

            fixture.Customize<Epic>(ComposerTransformation);
        }
    }
}
