using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    public static class DevelopmentPlanObjects
    {
        public static By DevelopmentPlanLink => By.Id("Link");

        public static By WorkOffPlansTable => ByExtensions.DataTestId("workoffplans");

        public static By WorkOffPlansActionLink => By.LinkText("Add a Work-off Plan item");
    }
}
