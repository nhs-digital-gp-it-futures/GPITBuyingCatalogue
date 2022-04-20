using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EditSolution
{
    public static class FeaturesObjects
    {
        public static By FeatureInput(int id) => By.Id($"Feature{id:D2}");
    }
}
