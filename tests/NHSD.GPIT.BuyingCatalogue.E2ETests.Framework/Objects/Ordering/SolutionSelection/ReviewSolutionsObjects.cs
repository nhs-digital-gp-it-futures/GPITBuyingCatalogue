using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection
{
    public static class ReviewSolutionsObjects
    {
        public static By CatalogueSolutionSectionTitle => By.Id("catalogue-solutions-title");

        public static By AdditionalServicesSectionTitle => By.Id("additional-services-title");

        public static By AssociatedServicesSectionTitle => By.Id("associated-services-title");

        public static By ContinueButton => By.LinkText("Continue");
    }
}
