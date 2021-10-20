using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CommonAutoDataAttribute : AutoDataAttribute
    {
        public CommonAutoDataAttribute()
            : base(FixtureFactory.Create)
        {
        }
    }
}
