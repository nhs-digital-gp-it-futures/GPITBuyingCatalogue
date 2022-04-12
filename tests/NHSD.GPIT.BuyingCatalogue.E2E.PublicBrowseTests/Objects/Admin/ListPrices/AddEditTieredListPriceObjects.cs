using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices
{
    internal static class AddEditTieredListPriceObjects
    {
        internal static By ProvisioningTypeInput => By.Id("selected-provisioning-type");

        internal static By CalculationTypeInput => By.Id("selected-calculation-type");

        internal static By UnitDescriptionInput => By.Id("UnitDescription");

        internal static By UnitDefinitionInput => By.Id("UnitDefinition");

        internal static By RangeDefinitionInput => By.Id("RangeDefinition");

        internal static By PublicationStatusInput => By.Id("selected-publication-status");

        internal static By PublishedInsetSection => ByExtensions.DataTestId("published-price-inset");

        internal static By TieredPriceTable => ByExtensions.DataTestId("tiered-price-table");

        internal static By DeclarativeBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_1");

        internal static By OnDemandBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_2");

        internal static By ProvisioningTypeInputError => By.Id("selected-provisioning-type-error");

        internal static By CalculationTypeInputError => By.Id("selected-calculation-type-error");

        internal static By UnitDescriptionInputError => By.Id("UnitDescription-error");

        internal static By RangeDefinitionInputError => By.Id("RangeDefinition-error");

        internal static By PublicationStatusInputError => By.Id("selected-publication-status-error");

        internal static By AddTierLink => By.LinkText("Add a pricing tier");

        internal static By DeletePriceLink => By.LinkText("Delete list price");

        internal static By DeleteTieredPriceTierLink => By.LinkText("Delete pricing tier");

        internal static By EditTierPriceLink(int tierIndex) => ByExtensions.DataTestId($"edit-tier-price-{tierIndex}");
    }
}
