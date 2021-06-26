using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CommonAutoDataAttribute : AutoDataAttribute
    {
        public CommonAutoDataAttribute() :
            base(() => new Fixture().Customize(
                new CompositeCustomization(
                    new AutoMoqCustomization(),                    
                    new IgnoreCircularReferenceCustomisation(),                    
                    new CallOffIdCustomization(),
                    new CatalogueItemIdCustomization(),

                    // TODO: causes test run initialization slow down
                    new SolutionCustomization(),
                    new SupplierCustomization(),
                    new CataloguePriceCustomization()
                    )))
        {
        }
    }
}
