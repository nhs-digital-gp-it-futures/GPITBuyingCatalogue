﻿using AutoFixture;
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
                    new CallOffIdCustomization(),
                    new CatalogueItemIdCustomization(),

                    // TODO: causes test run initialization slow down
                    new IgnoreCircularReferenceCustomisation(),
                    new SolutionCustomization()
                    //new SupplierCustomization()                    
                    //new CataloguePriceCustomization()
                    )))
        {
        }
    }
}
