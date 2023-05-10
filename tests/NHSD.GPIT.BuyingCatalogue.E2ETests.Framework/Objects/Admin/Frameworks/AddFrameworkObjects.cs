using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.Frameworks;

public static class AddFrameworkObjects
{
    public static By FrameworkNameInput => By.Id("Name");

    public static By IsLocalFundingOnlyInput => By.Id("is-local-funding-only");
}
