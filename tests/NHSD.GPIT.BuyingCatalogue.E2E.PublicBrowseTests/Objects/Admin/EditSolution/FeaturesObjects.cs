using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.EditSolution
{
    internal static class FeaturesObjects
    {
        internal static By FeatureInput(int id) => By.Id($"Feature{id:D2}");
    }
}
