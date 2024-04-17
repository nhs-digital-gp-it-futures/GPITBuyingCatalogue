using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public class SubstituteRelaySpecimenBuilder<T> : CompositeSpecimenBuilder
    {
        public SubstituteRelaySpecimenBuilder()
            : base(new SubstituteRelay(new ExactTypeSpecification(typeof(T))))
        {
        }
    }
}
