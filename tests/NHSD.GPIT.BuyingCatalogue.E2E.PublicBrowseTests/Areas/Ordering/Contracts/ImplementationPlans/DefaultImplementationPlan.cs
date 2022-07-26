using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.ImplementationPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.ImplementationPlans
{
    public class DefaultImplementationPlan : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int OrderId = 90001;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public DefaultImplementationPlan(LocalWebApplicationFactory factory)
            : base(factory, typeof(ImplementationPlanController), nameof(ImplementationPlanController.DefaultImplementationPlan), Parameters)
        {
        }

        [Fact]
        public void DefaultImplementationPlan_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void DefaultImplementationPlan_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ImplementationPlanController),
                nameof(ImplementationPlanController.DefaultImplementationPlan)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ImplementationPlanObjects.UseDefaultMilestonesError,
                $"Error:{DefaultImplementationPlanModelValidator.NoSelectionErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void DefaultImplementationPlan_SelectYes_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            var context = GetEndToEndDbContext();

            var defaultPlan = context.ImplementationPlans.Single(x => x.IsDefault);
            var contract = context.Contracts.Single(x => x.OrderId == OrderId);

            contract.ImplementationPlanId.Should().Be(defaultPlan.Id);
        }

        [Fact]
        public void DefaultImplementationPlan_SelectNo_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(DefaultImplementationPlanModel.NoOptionText);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ImplementationPlanController),
                nameof(ImplementationPlanController.CustomImplementationPlan)).Should().BeTrue();

            var context = GetEndToEndDbContext();

            var defaultPlan = context.ImplementationPlans.Single(x => x.IsDefault);
            var contract = context.Contracts.Single(x => x.OrderId == OrderId);

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
