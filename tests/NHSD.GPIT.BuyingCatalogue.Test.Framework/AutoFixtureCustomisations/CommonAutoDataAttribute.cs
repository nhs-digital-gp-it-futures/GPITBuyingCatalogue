using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

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
                    new SupplierCustomization()
                    
                    // TODO: This one in particular, yet no tests broken when its removed
                    //new CataloguePriceCustomization()
                    )))
        {
        }
    }
}
