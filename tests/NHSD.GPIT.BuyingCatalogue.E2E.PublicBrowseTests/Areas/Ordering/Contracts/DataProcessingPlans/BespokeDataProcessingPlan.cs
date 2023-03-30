using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DataProcessingPlans
{
    [Collection(nameof(OrderingCollection))]
    public class BespokeDataProcessingPlan : BuyerTestBase
    {
        private const int OrderId = 90001;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public BespokeDataProcessingPlan(LocalWebApplicationFactory factory)
            : base(factory, typeof(DataProcessingPlanController), nameof(DataProcessingPlanController.BespokeDataProcessingPlan), Parameters)
        {
        }

        [Fact]
        public void BespokeDataProcessingPlan_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DataProcessingPlanController),
                nameof(DataProcessingPlanController.Index)).Should().BeTrue();
        }

        [Fact]
        public void BespokeDataProcessingPlan_ClickSave_ExpectedResult()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
