using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class BuyingCatalogueDbContextCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<BuyingCatalogueDbContext> composer) => composer
                .OmitAutoProperties();

            fixture.Customize<BuyingCatalogueDbContext>(ComposerTransformation);
        }
    }
}
