using AutoFixture;
using AutoFixture.Kernel;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public class IgnoreCircularReferenceCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}