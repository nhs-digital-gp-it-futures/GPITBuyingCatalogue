using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.ImplementationPlans
{
    public class CustomImplementationPlan : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int OrderId = 90001;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public CustomImplementationPlan(LocalWebApplicationFactory factory)
            : base(factory, typeof(ImplementationPlanController), nameof(ImplementationPlanController.CustomImplementationPlan), Parameters)
        {
        }

        [Fact]
        public void CustomImplementationPlan_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ImplementationPlanController),
                nameof(ImplementationPlanController.DefaultImplementationPlan)).Should().BeTrue();
        }

        [Fact]
        public void CustomImplementationPlan_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            var context = GetEndToEndDbContext();

            var contract = context.Contracts.Single(x => x.OrderId == OrderId);
            var defaultPlan = context.ImplementationPlans.Single(x => x.IsDefault);

            contract.ImplementationPlanId.Should().NotBeNull();
            contract.ImplementationPlanId.Should().NotBe(defaultPlan.Id);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var contract = context.Contracts.FirstOrDefault(x => x.OrderId == OrderId);

            if (contract != null)
            {
                context.Contracts.Remove(contract);
            }

            context.SaveChanges();
        }
    }
}
