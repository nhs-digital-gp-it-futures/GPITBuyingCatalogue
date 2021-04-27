using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public class IgnoreCircularReferenceAutoDataAttribute : AutoDataAttribute
    {
        public IgnoreCircularReferenceAutoDataAttribute() : 
            base(new Fixture().Customize(new IgnoreCircularReferenceCustomisation()))
        {
        }
    }

    public class IgnoreCircularReferenceCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}