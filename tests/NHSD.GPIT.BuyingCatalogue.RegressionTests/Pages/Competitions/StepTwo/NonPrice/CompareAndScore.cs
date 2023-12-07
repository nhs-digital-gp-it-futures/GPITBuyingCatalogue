using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class CompareAndScore : PageBase
    {
        public CompareAndScore(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CompareAndScoreShortlistedSolutions(NonPriceElementType elementType)
        {
            string elementtype = elementType.ToString().ToLower();
            CommonActions.ClickLinkElement(NonPriceObjects.EditCompareAndScoreLink(elementtype));
            CatalogueSolutionScore();
            ReasonOfScore();
        }

        public void ReasonOfScore()
        {
            var textInput = TextGenerators.TextInput(100);
            CommonActions.EnterTextInTextBoxes(textInput);
            CommonActions.ClickSave();
        }

        public void CatalogueSolutionScore()
        {
            Driver.FindElements(By.XPath("//*[@class='nhsuk-input nhsuk-input--width-3']")).ToList().ForEach(element => element.SendKeys(RandonScore(1,5).ToString()));
        }

        public int RandonScore(int lowerRange = 0, int upperRange = int.MaxValue)
        {
            var random = new Random();
            var number = random.Next(lowerRange, upperRange);
            return number;
        }
    }
}
