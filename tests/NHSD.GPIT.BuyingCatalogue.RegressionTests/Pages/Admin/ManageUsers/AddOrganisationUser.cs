using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageUsers
{

    public class AddOrganisationUser : PageBase
    {
        public AddOrganisationUser(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddNewUser()
        {
            CommonActions.ClickLinkElement(UserObjects.AddUserLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add))
                .Should().BeTrue();
        }

        public void NewUserDetails(string organisation)
        {
            var user = GenerateUser.Generate();

            Driver.FindElement(AddUserObjects.Organisation).SendKeys(organisation);
            Driver.FindElement(AddUserObjects.FirstName).SendKeys(user.FirstName);
            Driver.FindElement(AddUserObjects.LastName).SendKeys(user.LastName);
            Driver.FindElement(AddUserObjects.UserEmail).SendKeys(user.EmailAddress);
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();
            CommonActions.HintText()
            .Should()
            .Be("Add a new user or edit the details for one that's already been created.".FormatForComparison());
        }
    }
}
