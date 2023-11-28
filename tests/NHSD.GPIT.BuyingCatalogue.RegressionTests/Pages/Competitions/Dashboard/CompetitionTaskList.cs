﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard
{
    public class CompetitionTaskList : PageBase
    {
        public CompetitionTaskList(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CompetitionServiceRecipientsTask()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.ServiceRecipientsLink);
            CommonActions.LedeText().Should().Be("Select the organisations that will receive the winning solution for this competition or upload them using a CSV file.".FormatForComparison());

            CommonActions.ClickExpander();
        }

        public void ContractLengthTask()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.ContractLengthLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CompetitionTaskListController),
                nameof(CompetitionTaskListController.ContractLength))
                .Should().BeTrue();
        }

        public void AwardCriteriaTask()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.AwardCriteriaLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CompetitionTaskListController),
                nameof(CompetitionTaskListController.AwardCriteria))
                .Should().BeTrue();
        }

        public void CalculatePriceTask()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.CalculatePriceLink);
            CommonActions.LedeText().Should().Be("Provide information to calculate the price for each of your shortlisted solutions. The calculation will be based on the quantity you want to order and the length of the contract.".FormatForComparison());
        }

        public void ViewResult()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.ViewResultLink);
            CommonActions.LedeText().Should().Be("Review the information you’ve added for this competition. If you’re happy, you can view your results and see your winning solution.".FormatForComparison());
        }

        public void AwardCriteriaWeightings()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.AwardCriteriaWeightingsLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CompetitionTaskListController),
                nameof(CompetitionTaskListController.Weightings))
                .Should().BeTrue();
        }

        public void NonPriceElements()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.NonPriceElementsLink);
            CommonActions.LedeText().Should().Be("Add at least 1 optional non-price element to help you score your shortlisted solutions, for example features, implementation, interoperability or service levels.".FormatForComparison());
        }
    }
}
