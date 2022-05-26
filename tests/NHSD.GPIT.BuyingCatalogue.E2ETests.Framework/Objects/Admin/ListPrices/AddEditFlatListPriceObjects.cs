using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices
{
    public static class AddEditFlatListPriceObjects
    {
        public static By ProvisioningTypeInput => By.Id("selected-provisioning-type");

        public static By CalculationTypeInput => By.Id("selected-calculation-type");

        public static By UnitDescriptionInput => By.Id("UnitDescription");

        public static By UnitDefinitionInput => By.Id("UnitDefinition");

        public static By PriceInput => By.Id("Price");

        public static By PublicationStatusInput => By.Id("selected-publication-status");

        public static By PerServiceRecipientBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_1");

        public static By DeclarativeBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_2");

        public static By OnDemandBillingPeriodInput => By.Id("conditional-SelectedProvisioningType_3");

        public static By ProvisioningTypeInputError => By.Id("selected-provisioning-type-error");

        public static By CalculationTypeInputError => By.Id("selected-calculation-type-error");

        public static By UnitDescriptionInputError => By.Id("UnitDescription-error");

        public static By PriceInputError => By.Id("Price-error");

        public static By PublicationStatusInputError => By.Id("selected-publication-status-error");

        public static By DeletePriceLink => By.LinkText("Delete list price");
    }
}
