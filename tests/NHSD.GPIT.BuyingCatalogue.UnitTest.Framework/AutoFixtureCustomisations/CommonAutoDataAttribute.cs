using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class CommonAutoDataAttribute : AutoDataAttribute
    {
        public CommonAutoDataAttribute()
            : base(FixtureFactory.Create)
        {
        }
    }
}
