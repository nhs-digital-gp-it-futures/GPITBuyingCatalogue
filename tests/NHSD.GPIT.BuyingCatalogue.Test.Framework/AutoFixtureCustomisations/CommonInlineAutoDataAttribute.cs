using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CommonInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public CommonInlineAutoDataAttribute(params object[] arguments)
            : base(new CommonAutoDataAttribute(), arguments)
        {
        }
    }
}
