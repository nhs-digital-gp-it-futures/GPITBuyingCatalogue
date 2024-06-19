using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions
{
    public class SupplierDetails : PageBase
    {
        public SupplierDetails(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddSupplierDetails(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.SupplierDetailsLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSupplierDetails))
                .Should().BeTrue();

            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }
    }
}
