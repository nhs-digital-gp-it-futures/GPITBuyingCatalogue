using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SolutionSelection
{
    public class SolutionShortlisted : PageBase
    {
        public SolutionShortlisted(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SolutionNotIncludedInShortlisting()
        {
            CommonActions.HintText().Should().Be("Provide a justification for why you've not included these solutions in your competition shortlist.".FormatForComparison());

            var textInput = TextGenerators.TextInput(100);
            CommonActions.EnterTextInTextBoxes(textInput);

            CommonActions.ClickSave();
        }

        public void ConfirmSolutions()
        {
            CommonActions.HintText().Should().Be("Review the solutions you’ve included in your shortlist and take them into a competition.".FormatForComparison());
            CommonActions.ClickSave();
        }
    }
}
