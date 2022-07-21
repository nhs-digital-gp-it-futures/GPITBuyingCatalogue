using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly
{
    public class SelectAssociatedServiceRecipientOnly : PageBase
    {
        public SelectAssociatedServiceRecipientOnly(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddServiceRecipient()
        {
            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ServiceRecipientsController),
              nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();
        }
    }
}
