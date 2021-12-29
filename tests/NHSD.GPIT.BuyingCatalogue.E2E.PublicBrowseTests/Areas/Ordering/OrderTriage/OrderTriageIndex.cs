using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.OrderTriage
{
    public sealed class OrderTriageIndex
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string OdsCode = "03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
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
            CommonActions
                .PageTitle()
                .Should()
                .BeEquivalentTo("What is the approximate value of the order you want to place?".FormatForComparison());

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
                nameof(OrderTriageController.NotSure))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Index_Selection_RedirectsToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("Under £40k");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.TriageSelection))
                .Should()
                .BeTrue();
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
