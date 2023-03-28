using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CommencementDate
{
    [Collection(nameof(OrderingCollection))]
    public sealed class CommencementDateAmendment : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
        private static readonly CallOffId CallOffId = new(90030, 2);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public CommencementDateAmendment(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CommencementDateController),
                  nameof(CommencementDateController.CommencementDate),
                  Parameters)
        {
        }

        [Fact]
        public void CommencementDate_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Timescales for Call-off Agreement - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommencementDateObjects.CommencementDateReadOnlyDisplay).Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommencementDateObjects.CommencementDateDayInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.CommencementDateMonthInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.CommencementDateYearInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.InitialPeriodInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.MaximumTermInput).Should().BeFalse();
            CommonActions.SaveButtonDisplayed().Should().BeFalse();
        }

        [Fact]
        public void CommencementDate_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_ClickContinue_ExpectedResult()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
