using System;
using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class InMemoryDbAutoDataAttribute : AutoDataAttribute
    {
        public InMemoryDbAutoDataAttribute()
            : base(() => FixtureFactory.Create(
                new BuyingCatalogueDbContextCustomization(),
                new InMemoryDbCustomization(Guid.NewGuid().ToString())))
        {
        }
    }
}
