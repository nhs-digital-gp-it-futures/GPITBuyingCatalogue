using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    public static class DevelopmentPlanObjects
    {
        public static By DevelopmentPlanLink => By.Id("Link");

        public static By WorkOffPlansTable => ByExtensions.DataTestId("workoffplans");

        public static By WorkOffPlansActionLink => By.LinkText("Add a Work-off Plan item");

        public static By SelectStandards => By.Id("SelectedStandard");

        public static By SelectStandardsError => By.Id("SelectedStandard-error");

        public static By Details => By.Id("Details");

        public static By DetailsError => By.Id("Details-error");

        public static By AgreeCompletionDateError => By.Id("edit-work-off-plan-error");
    }
}
