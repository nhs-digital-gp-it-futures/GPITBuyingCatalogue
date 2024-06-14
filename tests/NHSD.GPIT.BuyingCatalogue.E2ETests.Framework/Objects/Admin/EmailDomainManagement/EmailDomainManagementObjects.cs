using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EmailDomainManagement;

public class EmailDomainManagementObjects
{
    public static By HomeBreadcrumb => By.LinkText("Home");

    public static By NoDomainsText => ByExtensions.DataTestId("no-domains-text");

    public static By DomainsTable => ByExtensions.DataTestId("domains-table");

    public static By EmailDomainInput => By.Id("EmailDomain");

    public static By ManageAllowedEmailDomainLink => By.LinkText("Manage allowed email domains");

    public static By AddNewEmailDomainLink => By.LinkText("Add an email domain");
}
