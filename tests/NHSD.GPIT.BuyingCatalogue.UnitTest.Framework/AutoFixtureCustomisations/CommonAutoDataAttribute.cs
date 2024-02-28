using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class CommonAutoDataAttribute : AutoDataAttribute
    {
        public CommonAutoDataAttribute(MockingFramework mockingFramework = MockingFramework.Moq)
            : base(() => FixtureFactory.Create(mockingFramework))
        {
        }
    }
}
