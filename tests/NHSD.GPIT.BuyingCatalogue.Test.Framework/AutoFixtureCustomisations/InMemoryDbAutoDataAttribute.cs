using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class InMemoryDbAutoDataAttribute : AutoDataAttribute
    {
        // TODO: move specific test customizations out of CommonAutoDataAttribute
        // (HostingTypeSectionModelCustomization, ClientApplicationTypeSectionModelCustomization)
        public InMemoryDbAutoDataAttribute()
            : base(() => new Fixture().Customize(
                new CompositeCustomization(
                    new AutoMoqCustomization(),
                    new CallOffIdCustomization(),
                    new OrderCustomization(),
                    new CatalogueItemIdCustomization(),
                    new ControllerBaseCustomization(),
                    new IgnoreCircularReferenceCustomisation(),
                    new SolutionCustomization(),
                    new HostingTypeSectionModelCustomization(),
                    new ClientApplicationTypeSectionModelCustomization(),
                    new InMemoryDbCustomization(Guid.NewGuid().ToString()))))
        {
        }
    }
}
