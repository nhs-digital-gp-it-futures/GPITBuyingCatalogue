using System.Linq;
using AutoFixture;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal static class FixtureFactory
    {
        private static readonly ICustomization[] Customizations =
        {
            new ActionContextCustomization(),
            new ActionExecutedContextCustomization(),
            new AdditionalServiceCustomization(),
            new AspNetUserCustomization(),
            new AssociatedServiceCustomization(),
            new CallOffIdCustomization(),
            new CapabilityCategoryCustomization(),
            new CapabilityCustomization(),
            new CatalogueItemCapabilityCustomization(),
            new CatalogueItemCustomization(),
            new CatalogueItemEpicCustomization(),
            new CatalogueItemIdCustomization(),
            new CataloguePriceCustomization(),
            new CataloguePriceTierCustomization(),
            new ClientApplicationCustomization(),
            new ClientApplicationTypeSectionModelCustomization(),
            new ContactCustomization(),
            new ControllerBaseCustomization(),
            new ControllerCustomization(),
            new EpicCustomization(),
            new FrameworkSolutionCustomization(),
            new HostingTypeSectionModelCustomization(),
            new HttpClientCustomization(),
            new MarketingContactCustomization(),
            new MobileConnectionDetailsCustomization(),
            new MobileOperatingSystemsCustomization(),
            new OrderCustomization(),
            new OrderItemCustomization(),
            new OrderItemFundingCustomization(),
            new OrderItemPriceCustomization(),
            new OrderItemPriceTierCustomization(),
            new OrderItemRecipientCustomization(),
            new OrganisationCustomization(),
            new ServiceAvailabilityTimesCustomization(),
            new ServiceLevelAgreementCustomization(),
            new SolutionCustomization(),
            new StandardCapabilityCustomization(),
            new StandardsCustomization(),
            new SupplierContactCustomization(),
            new SupplierCustomization(),
            new WorkOffPlanCustomization(),
        };

        internal static IFixture Create() => Create(Customizations);

        internal static IFixture Create(params ICustomization[] customizations) =>
            new Fixture().Customize(new CompositeCustomization(Customizations.Union(customizations)));
    }
}
