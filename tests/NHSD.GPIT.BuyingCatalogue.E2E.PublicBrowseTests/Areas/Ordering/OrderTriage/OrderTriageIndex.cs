using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
    public sealed class OrderTriageIndex
        : BuyerTestBase
    {
        private const string InternalOrgId = "CG-03F";

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
            var organisation = context.Organisations.First(o => string.Equals(o.InternalIdentifier, InternalOrgId));

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
            CommonActions.ElementIsDisplayed(OrderTriageObjects.ReturnToDashboardLink).Should().BeTrue();

            CommonActions.ClickLinkElement(OrderTriageObjects.ProcurementHubLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ProcurementHubController),
                nameof(ProcurementHubController.Index)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.NotSure)).Should().BeTrue();

            CommonActions.ClickLinkElement(OrderTriageObjects.ReturnToDashboardLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();
        }

        [Theory]
        [InlineData(OrderTriageValue.Under40K)]
        [InlineData(OrderTriageValue.Between40KTo250K)]
        [InlineData(OrderTriageValue.Over250K)]
        public void Index_Selection_RedirectsToCorrectPage(
            OrderTriageValue option)
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

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                OrderTriageObjects.OrderValueError,
                "Error: Select the approximate value of your order, or ‘I’m not sure’ if you do not know")
                .Should()
                .BeTrue();
        }
    }
}
