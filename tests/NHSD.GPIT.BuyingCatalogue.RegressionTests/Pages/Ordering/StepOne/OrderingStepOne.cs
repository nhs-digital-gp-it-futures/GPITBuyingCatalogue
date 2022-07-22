﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
        }

        public void AddOrderDescription()
        {
            var description = TextGenerators.TextInputAddText(OrderDescription.DescriptionInput, 20);

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

        public void AddTimescaleForCallOffAgreement(OrderTriageValue orderTriage, CatalogueItemType itemType)
        {
            TextGenerators.DateInputAddDateSoon(
                CommencementDate.CommencementDateDayInput,
                CommencementDate.CommencementDateMonthInput,
                CommencementDate.CommencementDateYearInput);

            var upperRange = 0;
            var initialPeriod = TextGenerators.NumberInputAddRandomNumber(CommencementDate.InitialPeriodInput, 1, 6);

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

            TextGenerators.NumberInputAddRandomNumber(CommencementDate.MaximumTermInput, initialPeriod + 1, upperRange);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
