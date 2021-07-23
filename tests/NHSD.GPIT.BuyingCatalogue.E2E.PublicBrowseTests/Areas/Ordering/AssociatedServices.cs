using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class AssociatedServices
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CallOffId CallOffId = new(90004, 1);

        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" }, { "CallOffId", CallOffId.ToString() } };

        public AssociatedServices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void AssociatedServices_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddItemButton("Associated Service")).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddedItemsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.ContinueButton).Should().BeTrue();
        }

        [Fact]
        public void AssociatedServices_ClickAddSolution()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.AddItemButton("Associated Service"));

            CommonActions.PageLoadedCorrectGetIndex(typeof(AssociatedServicesController), nameof(AssociatedServicesController.SelectAssociatedService)).Should().BeTrue();
        }

        [Fact]
        public void AssociatedServices_ClickContinue()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(typeof(OrderController), nameof(OrderController.Order));
        }
    }
}
