using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates
{
    [Collection(nameof(OrderingCollection))]
    public class Review : BuyerTestBase
    {
        private const int OrderId = 91007;
        private const string InternalOrgId = "IB-QWO";
        private const string SolutionName = "E2E With Contact Multiple Prices";
        private const string AdditionalServiceName = "E2E No Contact Single Price Additional Service";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public Review(LocalWebApplicationFactory factory)
            : base(factory, typeof(DeliveryDatesController), nameof(DeliveryDatesController.Review), Parameters)
        {
        }

        [Fact]
        public void Review_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Review planned delivery dates - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.ReviewChangeDeliveryDateLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.ReviewEditDeliveryDatesLink(SolutionName)).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.ReviewEditDeliveryDatesLink(AdditionalServiceName)).Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Review_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void Review_ClickContinue_ExpectedResult()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void Review_ClickEditDeliveryDateLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(DeliveryDatesObjects.ReviewChangeDeliveryDateLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();
        }

        [Fact]
        public void Review_ClickEditSolutionLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(DeliveryDatesObjects.ReviewEditDeliveryDatesLink(SolutionName));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();
        }

        [Fact]
        public void Review_ClickEditServiceLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(DeliveryDatesObjects.ReviewEditDeliveryDatesLink(AdditionalServiceName));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();
        }
    }
}
