using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class ListPricesObjects
    {
        internal static By ListPriceTable => ByExtensions.DataTestId("solution-list-prices");

        internal static By ContinueLink => By.LinkText("Continue");

        internal static By PriceInput => By.Id("Price");

        internal static By UnitInput => By.Id("Unit");

        internal static By UnitDefinitionInput => By.Id("UnitDefinition");

        internal static By ProvisioningTypeInput => By.Id("edit-list-price");

        internal static By DeclarativeTimeInput => By.Id("conditional-SelectedProvisioningType_1");

        internal static By OnDemandTimeInput => By.Id("conditional-SelectedProvisioningType_2");

        internal static By PriceSummaryError => By.Id("Price-error");

        internal static By UnitSummaryError => By.Id("Unit-error");

        internal static By ProvisioningTypeSummaryError => By.Id("edit-list-price-error");

        internal static By DeleteListPriceCancelLink => By.LinkText("Cancel");
    }
}
