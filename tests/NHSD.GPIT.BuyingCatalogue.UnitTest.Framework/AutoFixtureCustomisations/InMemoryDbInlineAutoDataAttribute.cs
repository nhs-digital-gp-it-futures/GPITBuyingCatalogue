using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public sealed class InMemoryDbInlineAutoDataAttribute : InlineAutoDataAttribute
{
    public InMemoryDbInlineAutoDataAttribute(params object[] arguments)
        : base(new InMemoryDbAutoDataAttribute(), arguments)
    {
    }
}
