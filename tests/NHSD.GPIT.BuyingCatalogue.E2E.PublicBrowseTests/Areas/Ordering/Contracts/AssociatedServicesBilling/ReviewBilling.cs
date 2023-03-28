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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.AssociatedServicesBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.AssociatedServicesBilling
{
    [Collection(nameof(OrderingCollection))]
    public class ReviewBilling : BuyerTestBase, IDisposable
    {
        private const int OrderId = 90013;
        private const string InternalOrgId = "IB-QWO";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public ReviewBilling(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(AssociatedServicesBillingController),
                 nameof(AssociatedServicesBillingController.ReviewBilling),
                 Parameters)
        {
        }

        [Fact]
        public void ReviewBilling_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void ReviewBilling_ClickSave_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.ReviewBilling)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AssociatedServicesBillingObjects.UseDefaultBillingError).Should().BeTrue();
        }

        [Fact]
        public void ReviewBilling_SelectYes_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.SpecificRequirements)).Should().BeTrue();

            var flags = GetEndToEndDbContext().ContractFlags.First(x => x.OrderId == OrderId);

            flags.UseDefaultBilling.Should().BeTrue();
        }

        [Fact]
        public void ReviewBilling_SelectNo_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(ReviewBillingModel.NoOptionText);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.BespokeBilling)).Should().BeTrue();

            var flags = GetEndToEndDbContext().ContractFlags.First(x => x.OrderId == OrderId);

            flags.UseDefaultBilling.Should().BeFalse();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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
