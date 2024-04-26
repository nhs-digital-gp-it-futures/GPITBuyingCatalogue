using FluentAssertions;
using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions
{
    public class Features : PageBase
    {
        public Features(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddSolutionFeature(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.SolutionFeatureLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.Features))
                .Should().BeTrue();

            var textInput = TextGenerators.TextInput(100);
            EnterFeature(textInput);

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        public void EnterFeature(string x) =>
            Driver.FindElements(By.XPath("//*[@class='nhsuk-input']")).ToList().ForEach(element => element.SendKeys(x));
    }
}
