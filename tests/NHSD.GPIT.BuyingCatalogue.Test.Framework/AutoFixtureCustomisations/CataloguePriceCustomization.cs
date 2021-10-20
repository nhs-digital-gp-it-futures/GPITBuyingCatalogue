using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class CataloguePriceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CataloguePrice>(composer => composer
                .Without(cp => cp.CatalogueItem)
                .Without(cp => cp.CataloguePriceTiers)
                .With(cp => cp.CurrencyCode, "GBP")
                .With(cp => cp.CataloguePriceType, CataloguePriceType.Flat));
        }
    }
}
