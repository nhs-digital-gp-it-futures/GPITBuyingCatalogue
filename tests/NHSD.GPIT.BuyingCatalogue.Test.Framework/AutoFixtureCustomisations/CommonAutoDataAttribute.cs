using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CommonAutoDataAttribute : AutoDataAttribute
    {
        // TODO: move specific test customizations out of CommonAutoDataAttribute
        // (HostingTypeSectionModelCustomization, ClientApplicationTypeSectionModelCustomization)
        public CommonAutoDataAttribute()
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
                    new OrganisationCustomization(),
                    new ClientApplicationTypeSectionModelCustomization())))
        {
        }
    }
}
