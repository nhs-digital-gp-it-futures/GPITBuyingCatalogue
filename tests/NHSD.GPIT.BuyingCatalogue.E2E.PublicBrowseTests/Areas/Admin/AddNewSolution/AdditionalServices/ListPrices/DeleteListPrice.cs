using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AdditionalServices.ListPrices
{
    public sealed class DeleteListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int ListPriceId = 16;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
            { nameof(ListPriceId), ListPriceId.ToString() },
        };

        public DeleteListPrice(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.DeleteListPrice),
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
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.EditListPrice))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteListPrice_ClickDelete_ListPriceDeleted()
        {
            await using var context = GetEndToEndDbContext();

            var listPrice = new CataloguePrice
            {
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                PublishedStatus = PublicationStatus.Draft,
                IsLocked = false,
                PricingUnit = new PricingUnit
                {
                    TierName = "Test Tier",
                    Description = "per test declarative",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                Price = 123.4567M,
                LastUpdated = DateTime.UtcNow,
            };

            context.CataloguePrices.Add(listPrice);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>()
            {
                { nameof(SolutionId), SolutionId.ToString() },
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(ListPriceId), listPrice.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.DeleteListPrice),
                  parameters);

            var beforeCount = await context.CataloguePrices.CountAsync(cp => cp.CatalogueItemId == AdditionalServiceId);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.ManageListPrices))
                .Should()
                .BeTrue();

            (await context.CataloguePrices.CountAsync(cp => cp.CatalogueItemId == AdditionalServiceId)).Should().Be(beforeCount - 1);
        }
    }
}
