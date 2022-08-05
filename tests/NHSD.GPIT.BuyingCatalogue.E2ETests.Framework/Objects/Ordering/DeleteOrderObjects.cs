using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class DeleteOrderObjects
    {
        public static By WarningCallout => ByExtensions.DataTestId("delete-warning-callout");

        public static By SelectOptions => By.Id("selected-option");

        public static By SelectOptionError => By.Id("selected-option-error");
    }
}
