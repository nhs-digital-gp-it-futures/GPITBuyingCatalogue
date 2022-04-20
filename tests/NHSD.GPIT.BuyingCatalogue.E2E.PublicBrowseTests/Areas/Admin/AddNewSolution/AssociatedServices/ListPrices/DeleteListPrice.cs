using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AssociatedServices.ListPrices
{
    public sealed class DeleteListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int ListPriceId = 17;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly CatalogueItemId AssociatedServiceId = new(99998, "S-997");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AssociatedServiceId), AssociatedServiceId.ToString() },
            { nameof(ListPriceId), ListPriceId.ToString() },
        };

        public DeleteListPrice(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.DeleteListPrice),
                  Parameters)
        {
        }

        [Fact]
        public void DeleteListPrice_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(CommonSelectors.Header1)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.DeleteListPriceCancelLink)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void DeleteListPrice_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.ManageListPrices))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void DeleteListPrice_ClickCancelLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(ListPricesObjects.DeleteListPriceCancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.EditListPrice))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteListPrice_ClickDelete_ListPriceDeleted()
        {
            await using var context = GetEndToEndDbContext();
            var beforeCount = await context.CataloguePrices.CountAsync(cp => cp.CatalogueItemId == AssociatedServiceId);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.ManageListPrices))
                .Should()
                .BeTrue();

            (await context.CataloguePrices.CountAsync(cp => cp.CatalogueItemId == AssociatedServiceId)).Should().Be(beforeCount - 1);
        }
    }
}
