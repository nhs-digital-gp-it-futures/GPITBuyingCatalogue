using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne
{
    public class OrderingStepOne : PageBase
    {
        public OrderingStepOne(IWebDriver driver, CommonActions commonActions)
           : base(driver, commonActions)
        {
            TextGenerators = new TextGenerators(driver);
        }

        public TextGenerators TextGenerators { get; set; }

       /* public void StepOnePrepareOrder()
        {
            OrderingPages.TaskList.OrderDescriptionTask();
            AddOrderDescription();

            OrderingPages.TaskList.CallOffOrderingPartyContactDetailsTask();
            AddCallOffOrderingPartyContactDetails();

            OrderingPages.TaskList.SupplierInformationAndContactDetailsTask();
        }*/

        public void AddOrderDescription()
        {
            var description = TextGenerators.TextInputAddText(OrderDescription.DescriptionInput, 100);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AddCallOffOrderingPartyContactDetails()
        {
            TextGenerators.TextInputAddText(CalloffPartyInformation.FirstNameInput, 10);
            TextGenerators.TextInputAddText(CalloffPartyInformation.LastNameInput, 10);
            TextGenerators.TextInputAddText(CalloffPartyInformation.PhoneNumberInput, 10);
            TextGenerators.EmailInputAddText(CalloffPartyInformation.EmailAddressInput, 20);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(OrderController),
               nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
