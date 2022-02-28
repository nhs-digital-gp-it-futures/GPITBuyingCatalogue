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
    public sealed class OrderTriageIndex
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderTriageIndex(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(OrderTriageController),
                 nameof(OrderTriageController.Index),
                 Parameters)
        {
        }

        [Fact]
        public void Index_AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.Single(o => string.Equals(o.InternalIdentifier, InternalOrgId));

            CommonActions
                .PageTitle()
                .Should()
                .BeEquivalentTo($"What is the approximate value of the order you want to place? - {organisation.Name}".FormatForComparison());

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(4);
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Index_ChooseNotSure_RedirectsToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("I'm not sure");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.NotSure)).Should().BeTrue();

            CommonActions.ElementIsDisplayed(OrderTriageObjects.ProcurementHubLink).Should().BeTrue();

            CommonActions.ClickLinkElement(OrderTriageObjects.ProcurementHubLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ProcurementHubController),
                nameof(ProcurementHubController.Index)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.NotSure)).Should().BeTrue();
        }

        [Theory]
        [InlineData(TriageOption.Under40k)]
        [InlineData(TriageOption.Between40kTo250k)]
        [InlineData(TriageOption.Over250k)]
        public void Index_Selection_RedirectsToCorrectPage(
            TriageOption option)
        {
            CommonActions.ClickRadioButtonWithValue(option.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.TriageSelection)).Should().BeTrue();

            Driver.Url.Contains(option.ToString()).Should().BeTrue();
        }

        [Fact]
        public void Index_NoSelection_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed();
            CommonActions.ErrorSummaryLinksExist();
        }
    }
}
