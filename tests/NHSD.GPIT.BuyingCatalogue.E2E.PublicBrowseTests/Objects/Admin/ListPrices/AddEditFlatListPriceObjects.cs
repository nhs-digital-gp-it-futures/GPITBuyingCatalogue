using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices
{
    internal static class AddEditFlatListPriceObjects
    {
        internal static By ProvisioningTypeInput => By.Id("selected-provisioning-type");

        internal static By CalculationTypeInput => By.Id("selected-calculation-type");

        internal static By UnitDescriptionInput => By.Id("UnitDescription");

        internal static By UnitDefinitionInput => By.Id("UnitDefinition");

        internal static By PriceInput => By.Id("Price");

        internal static By PublicationStatusInput => By.Id("selected-publication-status");

        internal static By DeclarativeBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_1");

        internal static By OnDemandBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_2");

        internal static By ProvisioningTypeInputError => By.Id("selected-provisioning-type-error");

        internal static By CalculationTypeInputError => By.Id("selected-calculation-type-error");

        internal static By UnitDescriptionInputError => By.Id("UnitDescription-error");

        internal static By PriceInputError => By.Id("Price-error");

        internal static By PublicationStatusInputError => By.Id("selected-publication-status-error");

        internal static By DeletePriceLink => By.LinkText("Delete list price");
    }
}
