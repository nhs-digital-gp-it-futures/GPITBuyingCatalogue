using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class ListPricesObjects
    {
        public static By ListPriceTable => ByExtensions.DataTestId("solution-list-prices");

        public static By ContinueLink => By.LinkText("Continue");

        public static By PriceInput => By.Id("Price");

        public static By UnitInput => By.Id("Unit");

        public static By UnitDefinitionInput => By.Id("UnitDefinition");

        public static By ProvisioningTypeInput => By.Id("edit-list-price");

        public static By DeclarativeTimeInput => By.Id("conditional-SelectedProvisioningType_1");

        public static By OnDemandTimeInput => By.Id("conditional-SelectedProvisioningType_2");

        public static By PriceSummaryError => By.Id("Price-error");

        public static By UnitSummaryError => By.Id("Unit-error");

        public static By ProvisioningTypeSummaryError => By.Id("edit-list-price-error");

        public static By DeleteListPriceCancelLink => By.LinkText("Cancel");
    }
}
