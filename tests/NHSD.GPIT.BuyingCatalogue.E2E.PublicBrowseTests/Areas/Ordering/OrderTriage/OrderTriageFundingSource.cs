using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Extensions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.OrderTriage
{
    public sealed class OrderTriageFundingSource : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderTriageFundingSource(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(OrderTriageController),
                 nameof(OrderTriageController.TriageFunding),
                 Parameters)
        {
        }

        [Fact]
        public void OrderTriageFundingSource_AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.Single(o => string.Equals(o.InternalIdentifier, InternalOrgId));

            CommonActions.PageTitle().Should().Be($"What funding source are you using to pay for this order? - {organisation.Name}".FormatForComparison());
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(OrderTriageObjects.FundingSource).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderTriageObjects.FundingInset).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderTriageObjects.ProcurementHubActionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeTrue();
        }

        [Fact]
        public void OrderTriageFundingSource_ContactProcurementHub()
        {
            CommonActions.ClickLinkElement(OrderTriageObjects.ProcurementHubActionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(WebApp.Controllers.ProcurementHubController),
                nameof(WebApp.Controllers.ProcurementHubController.Index))
                .Should().BeTrue();
        }

        [Fact]
        public void OrderTriageFundingSource_ClickInset_TextDisplayed()
        {
            CommonActions.ElementIsDisplayed(OrderTriageObjects.FundingInsetText).Should().BeFalse();

            CommonActions.ClickLinkElement(OrderTriageObjects.FundingInsetLink);

            CommonActions.ElementIsDisplayed(OrderTriageObjects.FundingInsetText).Should().BeTrue();
        }

        [Fact]
        public void OrderTriageFundingSource_NoSelection_Submit_Error()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                OrderTriageObjects.FundingSourceError,
                "Error: Select a funding source")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void OrderTriageFundingSource_ValidSelection_Submit_Redirects()
        {
            CommonActions.ClickRadioButtonWithText("Local funding");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart))
                .Should()
                .BeTrue();

            Driver.GetQueryValue("fundingSource").Should().Be(ServiceContracts.Enums.FundingSource.Local.ToString());
        }

        [Fact]
        public void OrderTriageFundingSource_Preselected_PrepopulatesFundingSource()
        {
            var fundingSource = ServiceContracts.Enums.FundingSource.Central;

            var queryParameters = new Dictionary<string, string>()
            {
                { nameof(fundingSource), fundingSource.ToString() },
            };

            NavigateToUrl(
                typeof(OrderTriageController),
                nameof(OrderTriageController.TriageFunding),
                Parameters,
                queryParameters);

            CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(1);

            Driver.GetQueryValue("fundingSource").Should().Be(fundingSource.ToString());
        }

        [Fact]
        public void OrderTriageFundingSource_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.TriageSelection))
                .Should().BeTrue();
        }
    }
}
