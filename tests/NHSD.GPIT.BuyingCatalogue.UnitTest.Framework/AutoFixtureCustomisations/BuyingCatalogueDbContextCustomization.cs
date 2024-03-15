using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    [ExcludesAutoCustomization]
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
