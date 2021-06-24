using AutoFixture.NUnit3;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CommonInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public CommonInlineAutoDataAttribute(params object[] arguments)
            : base(FixtureFactory.Create, arguments)
        {
        }
    }
}
