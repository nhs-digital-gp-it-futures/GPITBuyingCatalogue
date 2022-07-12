using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class CataloguePriceTierCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CataloguePriceTier> composer) => composer
                .Without(pt => pt.UpperRange)
                .Without(pt => pt.LowerRange)
                .Without(pt => pt.CataloguePriceId)
                .Without(pt => pt.CataloguePrice);

            fixture.Customize<CataloguePriceTier>(ComposerTransformation);
        }
    }
}
