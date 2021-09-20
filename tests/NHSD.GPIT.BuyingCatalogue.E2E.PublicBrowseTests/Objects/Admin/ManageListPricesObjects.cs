using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class ManageListPricesObjects
    {
        internal static By ListPriceTable => ByExtensions.DataTestId("solution-list-prices");

        internal static By ContinueLink => By.LinkText("Continue");
    }
}
