using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class AdditionalServices
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CallOffId CallOffId = new(90005, 1);

        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" }, { "CallOffId", CallOffId.ToString() } };

        public AdditionalServices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServices_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddItemButton("Additional Service")).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddedItemsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.ContinueButton).Should().BeTrue();
        }

        [Fact]
        public void AdditionalServices_ClickAddAdditionalService()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.AddItemButton("Additional Service"));

            CommonActions.PageLoadedCorrectGetIndex(typeof(AdditionalServicesController), nameof(AdditionalServicesController.SelectAdditionalService)).Should().BeTrue();
        }

        [Fact]
        public void AdditionalServices_ClickContinue()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(typeof(OrderController), nameof(OrderController.Order));
        }
    }
}
