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
    public class SpecificRequirements : BuyerTestBase, IDisposable
    {
        private const int OrderId = 90013;
        private const string InternalOrgId = "IB-QWO";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { "fromBespoke", true.ToString() },
        };

        public SpecificRequirements(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(AssociatedServicesBillingController),
                 nameof(AssociatedServicesBillingController.SpecificRequirements),
                 Parameters)
        {
        }

        [Fact]
        public void SpecificRequirements_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.ReviewBilling)).Should().BeTrue();
        }

        [Fact]
        public void SpecificRequirements_ClickGoBackLink_FromBespoke_ExpectedResult()
        {
            NavigateToUrl(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.SpecificRequirements),
                Parameters,
                QueryParameters);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.BespokeBilling)).Should().BeTrue();
        }

        [Fact]
        public void SpecificRequirements_ClickSave_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.SpecificRequirements)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AssociatedServicesBillingObjects.HasSpecificRequirementsError).Should().BeTrue();
        }

        [Fact]
        public void SpecificRequirements_SelectNo_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(SpecificRequirementsModel.NoOptionText);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesBillingController),
                nameof(AssociatedServicesBillingController.BespokeRequirements)).Should().BeTrue();

            var flags = GetEndToEndDbContext().ContractFlags.First(x => x.OrderId == OrderId);

            flags.HasSpecificRequirements.Should().BeTrue();
        }

        [Fact]
        public void SpecificRequirements_SelectYes_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            var flags = GetEndToEndDbContext().ContractFlags.First(x => x.OrderId == OrderId);

            flags.HasSpecificRequirements.Should().BeFalse();
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
