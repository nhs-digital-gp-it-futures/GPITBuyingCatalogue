using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.Frameworks;

public static class DashboardFrameworkObjects
{
    public static By FrameworksTable => ByExtensions.DataTestId("frameworks-table");

    public static By AddNewFramwork => By.LinkText("Add a new framework");
}
