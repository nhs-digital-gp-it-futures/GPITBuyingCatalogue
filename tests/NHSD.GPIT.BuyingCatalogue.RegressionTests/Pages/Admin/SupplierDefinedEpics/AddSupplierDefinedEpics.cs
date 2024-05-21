using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.Frameworks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.SupplierDefinedEpics
{
    public class AddSupplierDefinedEpics : PageBase
    {
        public AddSupplierDefinedEpics(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddNewSupplierDefinedEpic()
        {
            CommonActions.ClickLinkElement(SupplierDefinedEpicsDashboardObjects.AddEpicLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.SelectCapabilities))
                .Should().BeTrue();
            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();
        }

        public void SupplierDefinedEpicDetails()
        {
            TextGenerators.OrganisationInputAddText(EditSupplierDefinedEpicDetailsObjects.SupplierDefinedEpicNameInput, 50);
            TextGenerators.TextInputAddText(EditSupplierDefinedEpicDetailsObjects.SupplierDefinedEpicDescriptionInput, 50);
            CommonActions.ClickRadioButtonWithText("Active");
            CommonActions.ClickSave();
        }

        public void SupplierDefinedEpicInformation()
        {
            CommonActions.HintText()
            .Should()
            .Be("Review the information for this supplier defined Epic or edit the details already added.".FormatForComparison());

            CommonActions.ClickSaveAndContinue();

            CommonActions.HintText()
            .Should()
            .Be("Add a supplier defined Epic or edit an existing one.".FormatForComparison());
        }
    }
}
