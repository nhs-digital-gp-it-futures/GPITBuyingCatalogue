using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class CataloguePriceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CataloguePrice>(
                c => c.With(cp => cp.PricingUnit, fixture.Create<PricingUnit>()));
        }
    }
}
