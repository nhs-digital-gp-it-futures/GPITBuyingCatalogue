using AutoFixture;
using AutoFixture.Kernel;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.AutoFixtureExtensions
{
    public static class AutoFixtureExtensions
    {
        public static int CreateIntWithRange(this ISpecimenContext context, int min, int max)
        {
            return (context.Create<int>() % (max - min + 1)) + min;
        }

        public static decimal CreateDecimalWithRange(this ISpecimenContext context, decimal min, decimal max)
        {
            return (context.Create<decimal>() % (max - min + 1)) + min;
        }
    }
}
