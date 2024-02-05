using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class CatalogueSolutionObjects
    {
        public static By SelectSolutionsAndServicesLink => By.LinkText("Solutions and services");

        public static By SelectSolution => By.Id("select-solution");

        public static By SelectSolutionErrorMessage => By.Id("select-solution-error");
    }
}
