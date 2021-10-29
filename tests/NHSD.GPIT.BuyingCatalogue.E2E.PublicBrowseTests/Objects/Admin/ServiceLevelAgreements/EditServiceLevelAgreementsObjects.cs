using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements
{
    internal static class EditServiceLevelAgreementsObjects
    {
        internal static By CatalogueSolutionType => By.CssSelector("p.nhsuk-heading-m");

        internal static By ServiceHoursTable => ByExtensions.DataTestId("service-hours-table");

        internal static By ContactDetailsTable => ByExtensions.DataTestId("contact-details-table");

        internal static By ServiceLevelsTable => ByExtensions.DataTestId("service-levels-table");
    }
}
