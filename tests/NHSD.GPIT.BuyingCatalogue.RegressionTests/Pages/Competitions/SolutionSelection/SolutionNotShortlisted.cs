using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.SolutionSelection
{
    public class SolutionNotShortlisted : PageBase
    {
        public SolutionNotShortlisted(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SolutionNotIncludedInShortlisting()
        {
            CommonActions.LedeText().Should().Be("Provide a justification for why these solutions were not included in your shortlist.".FormatForComparison());

            var textInput = TextGenerators.TextInput(100);
            EnterTextInAllTextBoxes(textInput);

            CommonActions.ClickSave();
        }

        public void EnterTextInAllTextBoxes(string x) =>
            Driver.FindElements(By.XPath("//*[@class=\"nhsuk-textarea\"]")).ToList().ForEach(element => element.SendKeys(x));
    }
}
