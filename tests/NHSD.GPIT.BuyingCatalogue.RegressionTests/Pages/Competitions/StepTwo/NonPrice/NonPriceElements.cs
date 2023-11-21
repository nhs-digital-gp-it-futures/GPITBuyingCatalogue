using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class NonPriceElements : PageBase
    {
        public NonPriceElements(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
            Features = new Features(driver, commonActions);
        }

        public Features Features { get; }

        public void AddNonPriceElements(NonPriceElementType elementType)
        {
            AddElements();
            if (elementType == NonPriceElementType.Feature)
            {
                Features.AddFeature();
            }
        }

        public void AddElements()
        {
            CommonActions.ClickLinkElement(NonPriceObjects.AddNonPriceElementLink);
            CommonActions.LedeText().Should().Be("You can add any or all optional non-price elements to help you score your shortlisted solutions.".FormatForComparison());
        }

        public void NonPriceElementsPage()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CompetitionNonPriceElementsController),
                nameof(CompetitionNonPriceElementsController.AddNonPriceElement))
                .Should().BeTrue();
            CommonActions.ClickLinkElement(NonPriceObjects.AddNonPriceElementLink);
        }
    }
    }
}
