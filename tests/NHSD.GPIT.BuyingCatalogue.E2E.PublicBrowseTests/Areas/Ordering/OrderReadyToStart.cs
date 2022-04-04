using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrderReadyToStart
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
        };

        public OrderReadyToStart(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderController),
                  nameof(OrderController.ReadyToStart),
                  Parameters)
        {
        }

        [Fact]
        public void ReadyToStart_AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.Single(o => string.Equals(o.InternalIdentifier, InternalOrgId));

            CommonActions.PageTitle().Should().BeEquivalentTo($"Before you start an order - {organisation.Name}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderTriageObjects.ProcurementHubLink).Should().BeTrue();
        }

        [Fact]
        public void ReadyToStart_ClickProcurementHubLink_RedirectsToCorrectPage()
        {
            CommonActions.ClickLinkElement(OrderTriageObjects.ProcurementHubLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ProcurementHubController),
                nameof(ProcurementHubController.Index));

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart));
        }

        [Fact]
        public void ReadyToStart_ClickContinue_RedirectsToCorrectPage()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.NewOrder));
        }
    }
}
