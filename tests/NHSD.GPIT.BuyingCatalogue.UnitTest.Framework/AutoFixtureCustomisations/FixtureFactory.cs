﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.OdsOrganisations;

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
            new MoqControllerCustomization(),
            new MobileConnectionDetailsCustomization(),
            new MobileOperatingSystemsCustomization(),
            new ApplicationTypeCustomization(),
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
            new OrderRecipientCustomization(),
            new OrderCustomization(),
            new HostingTypeSectionModelCustomization(),
            new ApplicationTypeSectionModelCustomization(),
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
            new CompetitionCatalogueItemPriceCustomization(),
            new CompetitionCatalogueItemPriceTierCustomization(),
            new SolutionQuantityCustomization(),
            new ServiceQuantityCustomization(),
            new NonPriceElementsCustomization(),
            new OdsOrganisationCustomization(),
            new OrganisationRoleCustomization(),
            new OrganisationRelationshipCustomization(),
            new ContractCustomization(),
            new ContractBillingCustomization(),
            new ContractBillingItemCustomization(),
            new ImplementationPlanCustomization(),
            new ImplementationPlanMilestoneCustomization(),
            new Gen2CapabilityMappingModelCustomization(),
            new Gen2CapabilitiesCsvModelCustomization(),
            new Gen2EpicsCsvModelCustomization(),
        };

        internal static IFixture Create()
            => Create(Array.Empty<ICustomization>());

        internal static IFixture Create(params ICustomization[] customizations) =>
            new Fixture().Customize(new CompositeCustomization(CreateCustomizations().Concat(customizations)));

        private static IEnumerable<ICustomization> CreateCustomizations()
            => Customizations.ToList();
    }
}
