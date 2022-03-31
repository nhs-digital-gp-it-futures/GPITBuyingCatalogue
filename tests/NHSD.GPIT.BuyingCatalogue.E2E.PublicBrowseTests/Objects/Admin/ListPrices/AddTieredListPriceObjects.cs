using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices
{
    internal static class AddTieredListPriceObjects
    {
        internal static By ProvisioningTypeInput => By.Id("selected-provisioning-type");

        internal static By CalculationTypeInput => By.Id("selected-calculation-type");

        internal static By UnitDescriptionInput => By.Id("UnitDescription");

        internal static By UnitDefinitionInput => By.Id("UnitDefinition");

        internal static By RangeDefinitionInput => By.Id("RangeDefinition");

        internal static By DeclarativeBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_1");

        internal static By OnDemandBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_2");

        internal static By ProvisioningTypeInputError => By.Id("selected-provisioning-type-error");

        internal static By CalculationTypeInputError => By.Id("selected-calculation-type-error");

        internal static By UnitDescriptionInputError => By.Id("UnitDescription-error");

        internal static By RangeDefinitionInputError => By.Id("RangeDefinition-error");
    }
}
