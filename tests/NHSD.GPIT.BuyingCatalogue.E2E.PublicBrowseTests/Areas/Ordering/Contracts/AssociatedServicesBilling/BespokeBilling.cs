using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.AssociatedServicesBilling
{
    [Collection(nameof(OrderingCollection))]
    public class BespokeBilling : BuyerTestBase
    {
        private const int OrderId = 90013;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public BespokeBilling(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(AssociatedServicesBillingController),
                 nameof(AssociatedServicesBillingController.BespokeBilling),
                 Parameters)
        {
        }

        [Fact]
        public void BespokeBilling_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.ReviewBilling)).Should().BeTrue();
        }

        [Fact]
        public void BespokeBilling_ClickSave_ExpectedResult()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.SpecificRequirements)).Should().BeTrue();
        }
    }
}
