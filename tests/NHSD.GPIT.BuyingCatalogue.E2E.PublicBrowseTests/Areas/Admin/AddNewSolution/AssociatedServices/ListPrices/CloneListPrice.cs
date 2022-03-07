﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AssociatedServices.ListPrices
{
    public sealed class CloneListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int ListPriceId = 17;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AssociatedServiceId = new(99998, "S-997");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AssociatedServiceId), AssociatedServiceId.ToString() },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { "basedOn", $"{ListPriceId}" },
        };

        public CloneListPrice(LocalWebApplicationFactory factory)
            : base(factory, typeof(AssociatedServicesController), nameof(AssociatedServicesController.AddListPrice), Parameters, QueryParameters)
        {
        }

        [Fact]
        public void CloneListPrice_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPricesObjects.ProvisioningTypeInput).Should().BeTrue();
            CommonActions.ElementIsNotDisplayed(ListPricesObjects.OnDemandTimeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPricesObjects.PriceInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPricesObjects.UnitInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPricesObjects.UnitDefinitionInput).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            var basePrice = GetBasePrice();

            CommonActions.ElementTextEqualTo(ListPricesObjects.PriceInput, $"{basePrice.Price}");
            CommonActions.ElementTextEqualTo(ListPricesObjects.UnitInput, $"{basePrice.PricingUnit.Description}");
        }

        [Fact]
        public void CloneListPrice_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.ManageListPrices)).Should().BeTrue();
        }

        [Fact]
        public void CloneListPrice_Submit_CreatesNewListPrice()
        {
            var cataloguePrices = GetCataloguePrices();
            var basePrice = GetBasePrice();

            cataloguePrices.Should().NotContain(x => x.Price == basePrice.Price
                && x.CataloguePriceId != basePrice.CataloguePriceId);

            var unitDescription = TextGenerators.TextInputAddText(ListPricesObjects.UnitInput, 10);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.PublishListPrice)).Should().BeTrue();

            GetCataloguePrices().Should().Contain(x => x.PricingUnit.Description == unitDescription
                && x.CataloguePriceId != basePrice.CataloguePriceId);
        }

        [Fact]
        public void CloneListPrice_DuplicateListPrice_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var prices = GetCataloguePrices();
            var basePrice = GetBasePrice();

            prices.RemoveAll(x => x.Price == basePrice.Price
                && x.CataloguePriceId != basePrice.CataloguePriceId);

            context.SaveChanges();
        }

        private CataloguePrice GetBasePrice()
        {
            var prices = GetCataloguePrices();

            return prices.Single(x => x.CataloguePriceId == ListPriceId);
        }

        private List<CataloguePrice> GetCataloguePrices()
        {
            using var context = GetEndToEndDbContext();

            return context
                .CataloguePrices
                .Include(c => c.CatalogueItem)
                .Include(c => c.PricingUnit)
                .Where(x => x.CatalogueItemId == AssociatedServiceId)
                .ToList();
        }
    }
}
