﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Triage
{
    public class OrderingTriage : PageBase
    {
        public OrderingTriage(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectOrderTriage(OrderTriageValue option, bool identified = true)
        {
            SelectTriageOrderValue(option);
            SelectIdentifiedOrder(identified);
        }

        /// <summary>
        /// Select the order triage value.
        /// </summary>
        /// <param name="option">the Selected Triage Value, Defaults to Under 40k.</param>
        public void SelectTriageOrderValue(OrderTriageValue option = OrderTriageValue.Under40K)
        {
            CommonActions.ClickRadioButtonWithValue(option.ToString());

            CommonActions.ClickSave();

            switch (option)
            {
                case OrderTriageValue.NotSure:
                    SelectTriageOrderValue_NotSureCorrectPage();
                    break;

                default:
                    SelectTriageOrderValue_SelectValueCorrectPage(option);
                    break;
            }
        }

        /// <summary>
        /// Select if the user has Identified what they want to order.
        /// </summary>
        /// <param name="identified">True means yes, False means No, Defaults to true.</param>
        public void SelectIdentifiedOrder(bool identified = true)
        {
            var identifiedValue = identified ? "Yes" : "No";

            CommonActions.ClickRadioButtonWithText(identifiedValue);

            CommonActions.ClickSave();

            if (identified)
            {
                SelectIdentifiedOrder_YesCorrectPage();
            }
            else
            {
                SelectIdentifiedOrder_NoCorrectPage();
            }
        }

        /********************************************************************
         * SelectTriageOrderValue Functions
         * *****************************************************************/

        private void SelectTriageOrderValue_NotSureCorrectPage()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.NotSure)).Should().BeTrue();
        }

        private void SelectTriageOrderValue_SelectValueCorrectPage(OrderTriageValue option)
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.TriageSelection)).Should().BeTrue();

            Driver.Url.Contains(option.ToString()).Should().BeTrue();
        }

        /********************************************************************
         * SelectIdentifiedOrder Functions
         * *****************************************************************/

        private void SelectIdentifiedOrder_YesCorrectPage()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart)).Should().BeTrue();
        }

        private void SelectIdentifiedOrder_NoCorrectPage()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.StepsNotCompleted)).Should().BeTrue();
        }
    }
}
