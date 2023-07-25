using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SolutionSelection
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
            CommonActions.EnterTextInTextBoxes(textInput);

            CommonActions.ClickSave();
        }
    }
}
