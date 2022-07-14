﻿using System;
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
        private const string SupplierName = "NotEmis Health";

        public OrderingStepOne(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

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

        public void SelectSupplierWithOneContactWithoutCreatingContact()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, SupplierName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.SelectSupplier)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.ClickSave();

            //here there is one contact(not need to select) and we don't create new contact

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(OrderController),
               nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AddTimescaleForCallOffAgreement()
        {
            TextGenerators.DateInputAddDateSoon(
                CommencementDate.CommencementDateDayInput,
                CommencementDate.CommencementDateMonthInput,
                CommencementDate.CommencementDateYearInput);

            var initialPeriod = TextGenerators.NumberInputAddRandomNumber(CommencementDate.InitialPeriodInput, 1, 6);
            TextGenerators.NumberInputAddRandomNumber(CommencementDate.MaximumTermInput, initialPeriod + 1, 12);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
