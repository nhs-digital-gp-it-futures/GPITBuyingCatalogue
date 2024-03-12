using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class CommonInlineAutoDataWithFrameworkAttribute : InlineAutoDataAttribute
    {
        public CommonInlineAutoDataWithFrameworkAttribute(MockingFramework mockingFramework, params object[] arguments)
            : base(new CommonAutoDataAttribute(mockingFramework), arguments)
        {
        }
    }
}
