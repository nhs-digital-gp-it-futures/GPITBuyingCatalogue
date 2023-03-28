using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates
{
    [Collection(nameof(OrderingCollection))]
    public class MatchDates : BuyerTestBase, IDisposable
    {
        private const int OrderId = 91007;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "002A999");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        public MatchDates(LocalWebApplicationFactory factory)
            : base(factory, typeof(DeliveryDatesController), nameof(DeliveryDatesController.MatchDates), Parameters)
        {
        }

        [Fact]
        public void MatchDates_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Match planned delivery dates for Additional Service - E2E No Contact Single Price Additional Service".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.MatchDates).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void MatchDates_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void MatchDates_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.MatchDates)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.MatchDatesError,
                $"Error: {MatchDatesModelValidator.NoSelectionMadeErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void MatchDates_SelectYes_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(MatchDatesModel.YesOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            var context = GetEndToEndDbContext();
            var expected = DateTime.Today.AddDays(1);

            context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId && x.CatalogueItemId == CatalogueItemId)
                .ForEach(x => x.DeliveryDate.Should().Be(expected));
        }

        [Fact]
        public void MatchDates_SelectNo_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(MatchDatesModel.NoOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            VerifyNoChangesMade();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var dayAfterTomorrow = DateTime.Today.AddDays(2);

            context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId && x.CatalogueItemId == CatalogueItemId)
                .ForEach(x => x.DeliveryDate = dayAfterTomorrow);

            context.SaveChanges();
        }

        private void VerifyNoChangesMade()
        {
            var context = GetEndToEndDbContext();
            var dayAfterTomorrow = DateTime.Today.AddDays(2);

            context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId && x.CatalogueItemId == CatalogueItemId)
                .ForEach(x => x.DeliveryDate.Should().Be(dayAfterTomorrow));
        }
    }
}
