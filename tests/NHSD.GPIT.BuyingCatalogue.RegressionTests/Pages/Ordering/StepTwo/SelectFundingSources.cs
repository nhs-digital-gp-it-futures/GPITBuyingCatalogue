using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.FundingSource;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    public class SelectFundingSources : PageBase
    {
        public SelectFundingSources(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddFundingSources()
        {
            var editLinksCount = CommonActions.NumberOfElementsDisplayed(FundingSources.EditLink);
            var names = new List<string>();
            if (editLinksCount == 1)
            {
                names.Add("Emis Web GP");
            }
            else if (editLinksCount == 2)
            {
                names.AddRange(new List<string> { "Anywhere Consult", "Anywhere Consult – Integrated Device" });
            }
            else
            {
                names.AddRange(new List<string> { "Emis Web GP", "Automated Arrivals – Engineering Half Day", "Automated Arrivals" });
            }

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
