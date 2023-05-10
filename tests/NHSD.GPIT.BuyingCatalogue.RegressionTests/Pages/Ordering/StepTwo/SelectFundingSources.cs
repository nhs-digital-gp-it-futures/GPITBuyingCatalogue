using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    public class SelectFundingSources : PageBase
    {
        public SelectFundingSources(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddFundingSources(string solutionName, bool isAssociatedServiceOnly, IEnumerable<string>? associatedServices, IEnumerable<string>? additionalServices)
        {
            var names = SelectSolutionAndServices.SelectSolutionServices(solutionName, isAssociatedServiceOnly, associatedServices, additionalServices);

            FundingSources(names);
        }

        public void AmendAddFundingSources(string solutionName, IEnumerable<string>? additionalServices)
        {
            var names = SelectSolutionAndServices.AmendSelectSolutionServices(solutionName, additionalServices);

            FundingSources(names);
        }

        private void FundingSources(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                CommonActions.ClickLinkElement(ByExtensions.DataTestId(name.Trim().Replace(' ', '-')));

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSource))
                    .Should().BeTrue();

                CommonActions.ClickFirstRadio();

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSources))
                    .Should().BeTrue();
            }

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
