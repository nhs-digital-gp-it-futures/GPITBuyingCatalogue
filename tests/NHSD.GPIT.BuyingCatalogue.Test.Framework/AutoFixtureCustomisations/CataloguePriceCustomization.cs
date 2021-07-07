using System;
using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class CataloguePriceCustomization : ICustomization
    {
        private static readonly Random Random = new();

        public void Customize(IFixture fixture)
        {
            fixture.Customize<CataloguePrice>(
                c => c.With(cp => cp.CurrencyCode, new[] { "GBP", "USD", "EUR" }[Random.Next(0, 3)])
                    .With(cp => cp.CataloguePriceType, CataloguePriceType.Flat)
                    .With(cp => cp.PricingUnit, fixture.Create<PricingUnit>())
                    .With(cp => cp.TimeUnit, fixture.Create<TimeUnit>()));
        }
    }
}
