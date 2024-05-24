using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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

        public static List<string> AllNonPriceElements()
        {
            var elementtype = Enum.GetValues(typeof(NonPriceElementType))
                        .Cast<NonPriceElementType>()
                        .Select(v => v.ToString())
                        .ToList().GetRange(1, 4);

            return elementtype;
        }

        public static List<string> MultipleNonPriceElements()
        {
            var elementtype = Enum.GetValues(typeof(NonPriceElementType))
                        .Cast<NonPriceElementType>()
                        .Select(v => v.ToString())
                        .ToList().GetRange(1, 2);

            return elementtype;
        }

        public void CompareAndScoreShortlistedSolutions(NonPriceElementType elementType)
        {
            if (elementType == NonPriceElementType.All)
            {
                var nonPriceElements = AllNonPriceElements();

                foreach (var element in nonPriceElements)
                {
                    if (element.ToLower() == NonPriceElementType.ServiceLevelAgreement.ToString().ToLower())
                    {
                        string slaElementType = "service-level";
                        CommonActions.ClickLinkElement(NonPriceObjects.EditCompareAndScoreLink(slaElementType));
                        CatalogueSolutionScore();
                        ReasonOfScore();
                    }
                    else
                    {
                        CommonActions.ClickLinkElement(NonPriceObjects.EditCompareAndScoreLink(element.ToLower()));
                        CatalogueSolutionScore();
                        ReasonOfScore();
                    }
                }
            }
            else if (elementType == NonPriceElementType.Multiple)
            {
                var nonPriceElements = MultipleNonPriceElements();
                foreach (var element in nonPriceElements)
                {
                    CommonActions.ClickLinkElement(NonPriceObjects.EditCompareAndScoreLink(element.ToLower()));
                    CatalogueSolutionScore();
                    ReasonOfScore();
                }
            }
            else
            {
                switch (elementType)
                {
                    case NonPriceElementType.ServiceLevelAgreement:
                        CommonActions.ClickLinkElement(NonPriceObjects.EditCompareAndScoreLink("service-level"));
                        break;
                    default:
                        CommonActions.ClickLinkElement(NonPriceObjects.EditCompareAndScoreLink(elementType.ToString().ToLower()));
                        break;
                }

                CatalogueSolutionScore();
                ReasonOfScore();
            }

            ReviewCompareAndScore();
        }

        public void ReasonOfScore()
        {
            var textInput = TextGenerators.TextInput(100);
            CommonActions.EnterTextInTextBoxes(textInput);
            CommonActions.ClickSave();
        }

        public void CatalogueSolutionScore()
        {
            Driver.FindElements(By.XPath("//*[@class='nhsuk-input nhsuk-input--width-3']")).ToList().ForEach(element => element.SendKeys(RandomScore(1,5).ToString()));
        }

        public int RandomScore(int lowerRange = 0, int upperRange = int.MaxValue)
        {
            var random = new Random();
            var number = random.Next(lowerRange, upperRange);
            return number;
        }

        public void ReviewCompareAndScore()
        {
            CommonActions.HintText().Should().Be("Compare and score shortlisted solutions based on the non-price elements you’ve added.".FormatForComparison());
            CommonActions.ClickSaveAndContinue();
        }
    }
}
