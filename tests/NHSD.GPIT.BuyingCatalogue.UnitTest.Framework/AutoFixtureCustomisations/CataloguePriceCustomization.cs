using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class CataloguePriceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CataloguePrice>(composer => composer
                .Without(cp => cp.CatalogueItem)
                .With(cp => cp.CurrencyCode, "GBP")
                .With(cp => cp.CataloguePriceType, CataloguePriceType.Flat));
        }
    }
}
