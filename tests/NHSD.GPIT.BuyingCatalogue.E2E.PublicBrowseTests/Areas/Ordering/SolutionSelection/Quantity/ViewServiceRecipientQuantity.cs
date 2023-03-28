using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Quantity
{
    [Collection(nameof(OrderingCollection))]
    public class ViewServiceRecipientQuantity : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
        private static readonly CallOffId CallOffId = new(90031, 2);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        public ViewServiceRecipientQuantity(LocalWebApplicationFactory factory)
            : base(factory, typeof(QuantityController), nameof(QuantityController.ViewOrderItemQuantity), Parameters)
        {
        }

        [Fact]
        public void ViewServiceRecipientQuantity_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Quantity of Catalogue Solution - E2E With Contact Multiple Prices".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(QuantityObjects.ViewServiceRecipientQuantity).Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ViewServiceRecipientQuantity_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        [Fact]
        public void ViewServiceRecipientQuantity_ClickContinue_ExpectedResult()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }
    }
}
