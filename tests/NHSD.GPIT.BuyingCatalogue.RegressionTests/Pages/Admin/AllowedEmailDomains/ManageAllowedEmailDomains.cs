using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EmailDomainManagement;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.AllowedEmailDomains
{
    public class ManageAllowedEmailDomains : PageBase
    {
        public ManageAllowedEmailDomains(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddNewAllowedEmailDomain()
        {
            CommonActions.ClickLinkElement(EmailDomainManagementObjects.AddNewEmailDomainLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.AddEmailDomain))
                .Should().BeTrue();

            AllowedEmailDomainDetails();

            CommonActions.ClickSaveAndContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
                .Should().BeTrue();
        }

        public void AllowedEmailDomainDetails()
        {
            const string emailDomin = ".net";
            string domainName = TextGenerators.TextInput(10);
            string validEmailDomain = $"@{domainName}{emailDomin}";

            CommonActions.ElementAddValue(EmailDomainManagementObjects.EmailDomainInput, validEmailDomain);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(EmailDomainManagementController),
                    nameof(EmailDomainManagementController.Index))
                .Should()
                .BeTrue();
        }
    }
}
