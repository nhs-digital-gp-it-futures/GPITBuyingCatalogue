using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.OrderTriage
{
    [Collection(nameof(OrderingCollection))]
    public sealed class OrderTriageSelection
        : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
        private const OrderTriageValue Option = OrderTriageValue.Under40K;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(Option), Option.ToString() },
        };

        public OrderTriageSelection(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(OrderTriageController),
                 nameof(OrderTriageController.TriageSelection),
                 Parameters)
        {
        }

        [Fact]
        public void TriageSelection_AllSectionsDisplayed()
        {
            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(2);
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Theory]
        [InlineData(OrderTriageValue.Under40K, "Have you identified what you want to order?")]
        [InlineData(OrderTriageValue.Between40KTo250K, "Have you carried out a competition using the Buying Catalogue?")]
        [InlineData(OrderTriageValue.Over250K, "Have you sent out Invitations to Tender to suppliers?")]
        public void TriageSelection_ShowsCorrectTitle(OrderTriageValue option, string title)
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.First(o => string.Equals(o.InternalIdentifier, InternalOrgId));

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(option), option.ToString() },
            };

            NavigateToUrl(
                 typeof(OrderTriageController),
                 nameof(OrderTriageController.TriageSelection),
                 parameters);

            CommonActions.PageTitle().Should().BeEquivalentTo($"{title} - {organisation.Name}".FormatForComparison());
        }

        [Theory]
        [InlineData(OrderTriageValue.Under40K, "As your order is under £40k, you can execute a Direct Award. Any Catalogue Solution or service on the Buying Catalogue can be procured without carrying out a competition.")]
        [InlineData(OrderTriageValue.Between40KTo250K, "As your order is between £40k and £250k, you should have executed an On-Catalogue Competition to identify the Catalogue Solution that best meets your needs.")]
        [InlineData(OrderTriageValue.Over250K, "As your order is over £250k, you should have executed an Off-Catalogue Competition to identify the Catalogue Solution that best meets your needs.")]
        public void TriageSelection_ShowsCorrectAdvice(OrderTriageValue option, string advice)
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(option), option.ToString() },
            };

            NavigateToUrl(
                 typeof(OrderTriageController),
                 nameof(OrderTriageController.TriageSelection),
                 parameters);

            CommonActions.LedeText().Should().BeEquivalentTo(advice.FormatForComparison());
        }

        [Theory]
        [InlineData(OrderTriageValue.Under40K, true)]
        [InlineData(OrderTriageValue.Between40KTo250K, true)]
        [InlineData(OrderTriageValue.Over250K, false)]
        public void TriageSelection_ShowsInset(OrderTriageValue option, bool insetVisible)
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(option), option.ToString() },
            };

            NavigateToUrl(
                 typeof(OrderTriageController),
                 nameof(OrderTriageController.TriageSelection),
                 parameters);

            CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("triage-selection-inset")).Should().Be(insetVisible);
        }

        [Theory]
        [InlineData(OrderTriageValue.Under40K, "Orders with a value less than £40k")]
        [InlineData(OrderTriageValue.Between40KTo250K, "Orders with a value between £40k and £250k")]
        [InlineData(OrderTriageValue.Over250K, "Orders with a value over £250k")]
        public void StepsNotCompleted_LoadsCorrectPage(OrderTriageValue option, string title)
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.First(o => string.Equals(o.InternalIdentifier, InternalOrgId));

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(option), option.ToString() },
            };

            NavigateToUrl(
                typeof(OrderTriageController),
                nameof(OrderTriageController.StepsNotCompleted),
                parameters);

            CommonActions.PageTitle().Should().BeEquivalentTo($"{title} - {organisation.Name}".FormatForComparison());
        }

        [Theory]
        [InlineData(OrderTriageValue.Under40K)]
        [InlineData(OrderTriageValue.Between40KTo250K)]
        [InlineData(OrderTriageValue.Over250K)]
        public void TriageSelection_No_RedirectsToCorrectPage(OrderTriageValue option)
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(option), option.ToString() },
            };

            NavigateToUrl(
                typeof(OrderTriageController),
                nameof(OrderTriageController.TriageSelection),
                parameters);

            CommonActions.ClickRadioButtonWithText("No");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.StepsNotCompleted)).Should().BeTrue();

            CommonActions.ElementIsDisplayed(OrderTriageObjects.ProcurementHubLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderTriageObjects.ReturnToDashboardLink).Should().BeTrue();

            CommonActions.ClickLinkElement(OrderTriageObjects.ProcurementHubLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ProcurementHubController),
                nameof(ProcurementHubController.Index)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.StepsNotCompleted)).Should().BeTrue();

            CommonActions.ClickLinkElement(OrderTriageObjects.ReturnToDashboardLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();
        }

        [Fact]
        public void TriageSelection_Yes_RedirectsToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart)).Should().BeTrue();
        }

        [Fact]
        public void TriageSelection_NoSelection_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                OrderTriageObjects.DueDiligenceError,
                "Error: Select yes if you’ve identified what you want to order")
                .Should()
                .BeTrue();
        }
    }
}
