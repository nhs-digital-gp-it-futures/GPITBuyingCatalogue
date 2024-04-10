using AutoFixture.AutoMoq;
using AutoFixture.Kernel;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public class MoqRelaySpecimenBuilder<T> : CompositeSpecimenBuilder
    {
        public MoqRelaySpecimenBuilder()
            : base(new MockRelay(new ExactTypeSpecification(typeof(T))))
        {
        }
    }
}
