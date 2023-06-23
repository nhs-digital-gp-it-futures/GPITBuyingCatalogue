using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal static class FixtureFactory
    {
        private static readonly ICustomization[] Customizations =
        {
            new AutoMoqCustomization(),
            new AspNetUserCustomization(),
            new CapabilityCategoryCustomization(),
            new CapabilityCustomization(),
            new EpicCustomization(),
            new CatalogueItemEpicCustomization(),
            new CatalogueItemCustomization(),
            new CatalogueItemCapabilityCustomization(),
            new CatalogueItemIdCustomization(),
            new CataloguePriceCustomization(),
            new CataloguePriceTierCustomization(),
            new CallOffIdCustomization(),
            new SupplierContactCustomization(),
            new SupplierCustomization(),
            new MarketingContactCustomization(),
            new FrameworkSolutionCustomization(),
            new MobileConnectionDetailsCustomization(),
            new MobileOperatingSystemsCustomization(),
            new ClientApplicationCustomization(),
            new AdditionalServiceCustomization(),
            new AssociatedServiceCustomization(),
            new SolutionCustomization(),
            new StandardsCustomization(),
            new StandardCapabilityCustomization(),
            new OrderItemRecipientCustomization(),
            new OrderItemFundingCustomization(),
            new OrderItemCustomization(),
            new OrderItemPriceCustomization(),
            new OrderItemPriceTierCustomization(),
            new OrganisationCustomization(),
            new OrderCustomization(),
            new ControllerCustomization(),
            new ControllerBaseCustomization(),
            new HostingTypeSectionModelCustomization(),
            new ClientApplicationTypeSectionModelCustomization(),
            new ServiceLevelAgreementCustomization(),
            new ActionContextCustomization(),
            new ActionExecutedContextCustomization(),
            new ActionExecutingContextCustomization(),
            new ServiceAvailabilityTimesCustomization(),
            new WorkOffPlanCustomization(),
            new HttpClientCustomization(),
            new AbstractValidatorCustomizations(),
            new RecursiveErrorsCustomisation(),
            new RecipientDateModelCustomization(),
            new SelectDateModelCustomization(),
            new BlobServiceClientCustomization(),
            new OdsSettingsCustomization(),
            new CompetitionCustomization(),
            new CompetitionSolutionCustomization(),
            new RequiredServiceCustomization(),
        };

        internal static IFixture Create() => Create(Customizations);

        internal static IFixture Create(params ICustomization[] customizations) =>
            new Fixture().Customize(new CompositeCustomization(Customizations.Union(customizations)));
    }
}
