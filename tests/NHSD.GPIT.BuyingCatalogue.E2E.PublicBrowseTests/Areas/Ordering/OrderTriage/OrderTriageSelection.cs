using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.OrderTriage
{
    public sealed class OrderTriageSelection
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "03F";
        private const TriageOption Option = TriageOption.Under40k;

        private static readonly Dictionary<string, string> Parameters =
            new()
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
        [InlineData(TriageOption.Under40k, "Have you identified what you want to order?")]
        [InlineData(TriageOption.Between40kTo250k, "Have you carried out a competition using the Buying Catalogue?")]
        [InlineData(TriageOption.Over250k, "Have you sent out Invitations to Tender to suppliers?")]
        public void TriageSelection_ShowsCorrectTitle(TriageOption option, string title)
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.Single(o => string.Equals(o.InternalIdentifier, InternalOrgId));

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
        [InlineData(TriageOption.Under40k, "As your order is under £40k, you should have executed a Direct Award. You can procure any Catalogue Solution or service on the Buying Catalogue without carrying out a competition.")]
        [InlineData(TriageOption.Between40kTo250k, "As your order is between £40k and £250k, you should have executed an On-Catalogue Competition to identify the Catalogue Solution that best meets your needs.")]
        [InlineData(TriageOption.Over250k, "As your order is over £250k, you should have executed an Off-Catalogue Competition to identify the Catalogue Solution that best meets your needs.")]
        public void TriageSelection_ShowsCorrectAdvice(TriageOption option, string advice)
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
        [InlineData(TriageOption.Under40k, "Orders with a value less than £40k")]
        [InlineData(TriageOption.Between40kTo250k, "Orders with a value between £40k and £250k")]
        [InlineData(TriageOption.Over250k, "Orders with a value over £250k")]
        public void StepsNotCompleted_LoadsCorrectPage(TriageOption option, string title)
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.Single(o => string.Equals(o.InternalIdentifier, InternalOrgId));

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
        [InlineData(TriageOption.Under40k)]
        [InlineData(TriageOption.Between40kTo250k)]
        [InlineData(TriageOption.Over250k)]
        public void TriageSelection_No_RedirectsToCorrectPage(TriageOption option)
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

            CommonActions.ClickLinkElement(OrderTriageObjects.ProcurementHubLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ProcurementHubController),
                nameof(ProcurementHubController.Index)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.StepsNotCompleted)).Should().BeTrue();
        }

        [Fact]
        public void TriageSelection_Yes_RedirectsToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.TriageFunding)).Should().BeTrue();
        }

        [Fact]
        public void TriageSelection_NoSelection_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed();
            CommonActions.ErrorSummaryLinksExist();
        }
    }
}
