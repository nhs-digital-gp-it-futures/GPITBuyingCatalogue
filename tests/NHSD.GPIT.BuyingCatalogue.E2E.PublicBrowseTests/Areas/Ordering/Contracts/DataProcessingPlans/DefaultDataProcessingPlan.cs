using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DataProcessing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DataProcessingPlans
{
    [Collection(nameof(OrderingCollection))]
    public class DefaultDataProcessingPlan : BuyerTestBase, IDisposable
    {
        private const int OrderId = 90001;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public DefaultDataProcessingPlan(LocalWebApplicationFactory factory)
            : base(factory, typeof(DataProcessingPlanController), nameof(DataProcessingPlanController.Index), Parameters)
        {
        }

        [Fact]
        public void DefaultDataProcessingPlan_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void DefaultDataProcessingPlan_ClickContinue_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            var flags = GetEndToEndDbContext().ContractFlags.First(x => x.OrderId == OrderId);

            flags.UseDefaultDataProcessing.Should().BeTrue();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var contract = context.ContractFlags.FirstOrDefault(x => x.OrderId == OrderId);

            if (contract != null)
            {
                context.ContractFlags.Remove(contract);
            }

            context.SaveChanges();
        }
    }
}
