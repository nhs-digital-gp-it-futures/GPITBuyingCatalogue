﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne
{
    public class OrderingStepOne : PageBase
    {
        public OrderingStepOne(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddOrderDescription(string orderDescription)
        {
            Driver.FindElement(OrderDescription.DescriptionInput).SendKeys(orderDescription);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AddCallOffOrderingPartyContactDetails()
        {
            TextGenerators.FirstNameInputAddText(CalloffPartyInformation.FirstNameInput, 10);
            TextGenerators.LastNameInputAddText(CalloffPartyInformation.LastNameInput, 10);
            TextGenerators.PhoneNumberInputAddText(CalloffPartyInformation.PhoneNumberInput, 10);
            TextGenerators.EmailInputAddText(CalloffPartyInformation.EmailAddressInput, 20);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(OrderController),
               nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AddTimescaleForCallOffAgreement(OrderTriageValue orderTriage, CatalogueItemType itemType)
        {
            TextGenerators.DateInputAddDateSoon(
                CommencementDateObjects.CommencementDateDayInput,
                CommencementDateObjects.CommencementDateMonthInput,
                CommencementDateObjects.CommencementDateYearInput);

            var upperRange = 0;
            var initialPeriod = TextGenerators.NumberInputAddRandomNumber(CommencementDateObjects.InitialPeriodInput, 1, 6);

            if (itemType == CatalogueItemType.AssociatedService)
            {
                upperRange = 36;
            }
            else
            {
                if (orderTriage == OrderTriageValue.Under40K)
                {
                    upperRange = 12;
                }
                else
                {
                    upperRange = 36;
                }
            }

            TextGenerators.NumberInputAddRandomNumber(CommencementDateObjects.MaximumTermInput, initialPeriod + 1, upperRange);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
