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
    public class ViewOrderItemQuantity : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
        private static readonly CallOffId CallOffId = new(90031, 2);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        public ViewOrderItemQuantity(LocalWebApplicationFactory factory)
            : base(factory, typeof(QuantityController), nameof(QuantityController.ViewOrderItemQuantity), Parameters)
        {
        }

        [Fact]
        public void ViewOrderItemQuantity_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Quantity of Additional Service - E2E Multiple Prices Additional Service".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(QuantityObjects.ViewOrderItemQuantity).Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ViewOrderItemQuantity_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        [Fact]
        public void ViewOrderItemQuantity_ClickContinue_ExpectedResult()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }
    }
}
