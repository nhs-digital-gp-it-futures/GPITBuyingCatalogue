using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices
{
    public static class ListPriceObjects
    {
        public static By ProvisioningTypeInput => By.Id("selected-provisioning-type");

        public static By UnitDescriptionInput => By.Id("UnitDescription");

        public static By UnitDefinitionInput => By.Id("UnitDefinition");

        public static By PriceInput => By.Id("InputPrice");

        public static By PublicationStatusInput => By.Id("selected-publication-status");

        public static By DeclarativeBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_1");

        public static By OnDemandBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_2");

        public static By ProvisioningTypeInputError => By.Id("selected-provisioning-type-error");

        public static By UnitDescriptionInputError => By.Id("UnitDescription-error");

        public static By PriceInputError => By.Id("InputPrice-error");

        public static By PublicationStatusInputError => By.Id("selected-publication-status-error");

        public static By DeletePriceLink => By.LinkText("Delete list price");

        public static By ProvisioningTypeRadioButtons => By.Name("SelectedProvisioningType");

        public static By DeclarativeQuantityCalculationRadioButtons => By.Id("declarative-quantity-calculation-type");

        public static By OnDemandQuantityCalculationRadioButtons => By.Id("on-demand-quantity-calculation-type");

        public static By CalculationTypeInput => By.Id("selected-calculation-type");

        public static By RangeDefinitionInput => By.Id("RangeDefinition");

        public static By PublishedInsetSection => ByExtensions.DataTestId("published-price-inset");

        public static By MaximumTiersInset => ByExtensions.DataTestId("maximum-tiers-reached-inset");

        public static By TieredPriceTable => ByExtensions.DataTestId("tiered-price-table");

        public static By CalculationTypeInputError => By.Id("selected-calculation-type-error");

        public static By RangeDefinitionInputError => By.Id("RangeDefinition-error");

        public static By AddTierLink => By.LinkText("Add a pricing tier");

        public static By DeleteTieredPriceTierLink => By.LinkText("Delete pricing tier");

        public static By EditTierPriceLink(int tierIndex) => ByExtensions.DataTestId($"edit-tier-price-{tierIndex}");
    }
}
