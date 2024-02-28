using System;
using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class InMemoryDbAutoDataAttribute : AutoDataAttribute
    {
        public InMemoryDbAutoDataAttribute(MockingFramework mockingFramework = MockingFramework.Moq)
            : base(() => FixtureFactory.Create(
                mockingFramework,
                new BuyingCatalogueDbContextCustomization(),
                new InMemoryDbCustomization(Guid.NewGuid().ToString(), mockingFramework),
                new UserManagerCustomization()))
        {
        }
    }
}
