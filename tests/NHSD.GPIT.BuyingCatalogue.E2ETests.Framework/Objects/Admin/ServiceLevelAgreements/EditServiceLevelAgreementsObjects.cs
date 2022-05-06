using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements
{
    public static class EditServiceLevelAgreementsObjects
    {
        public static By CatalogueSolutionType => By.CssSelector("p.nhsuk-heading-m");

        public static By ServiceHoursTable => ByExtensions.DataTestId("service-hours-table");

        public static By ContactDetailsTable => ByExtensions.DataTestId("contact-details-table");

        public static By ServiceLevelsTable => ByExtensions.DataTestId("service-levels-table");
    }
}
