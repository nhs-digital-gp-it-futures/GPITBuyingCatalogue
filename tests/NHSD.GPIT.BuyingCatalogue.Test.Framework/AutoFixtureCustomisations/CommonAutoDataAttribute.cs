using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public class CommonAutoDataAttribute : AutoDataAttribute
    {
        public CommonAutoDataAttribute() :
            base(() => new Fixture().Customize(
                new CompositeCustomization(new AutoMoqCustomization(),
                    new IgnoreCircularReferenceCustomisation(),
                    new SolutionCustomization())))
        {
        }
    }
}
