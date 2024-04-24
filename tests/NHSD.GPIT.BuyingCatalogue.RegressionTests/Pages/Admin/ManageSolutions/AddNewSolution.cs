using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions
{
    public class AddNewSolution : PageBase
    {
        public AddNewSolution(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddSolution()
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.AddSolutionLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AddCatalogueSolutionController),
                nameof(AddCatalogueSolutionController.Index))
                .Should().BeTrue();
        }

        public void AddSolutionDetails()
        {
            var solutionId = GetSolution();

            TextGenerators.OrganisationInputAddText(AddSolutionObjects.SolutionName, 255);
            ClickSupplierDropDownListWIthValue(solutionId);
            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        public void AddSolutionDescription(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.SolutionDescriptionLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.Description))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Marketing.CommonSelectors.Summary, 350);
            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Marketing.CommonSelectors.Description, 1000);
            TextGenerators.UrlInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.LinkTextBox, 50);

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        public void ClickSupplierDropDownListWIthValue(string value) =>
            new SelectElement(
            Driver.FindElement(AddSolutionObjects.SupplierName)).SelectByValue(value);

        private string GetSolution()
        {
            using var dbContext = Factory.DbContext;

            var solution = dbContext.Solutions
                .Select(x => x.CatalogueItemId).FirstOrDefault().ToString();
            var solutionId = solution.Split('-').First();

            return solutionId;
        }
    }
}
