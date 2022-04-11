using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ListPrices
{
    public static class TieredPriceTiersModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoTiers_SetsModelError(
            Solution solution,
            CataloguePrice price,
            TieredPriceTiersModel model,
            [Frozen] Mock<ISolutionListPriceService> service,
            TieredPriceTiersModelValidator validator)
        {
            model.SelectedPublicationStatus = PublicationStatus.Published;

            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>();
            solution.CatalogueItem.CataloguePrices.Add(price);

            model.CatalogueItemId = solution.CatalogueItemId;
            model.CataloguePriceId = price.CataloguePriceId;

            service.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage(TieredPriceTiersModelValidator.MissingTiersError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InvalidStartingRange_SetsModelError(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            TieredPriceTiersModel model,
            [Frozen] Mock<ISolutionListPriceService> service,
            TieredPriceTiersModelValidator validator)
        {
            model.SelectedPublicationStatus = PublicationStatus.Published;

            tier.LowerRange = 2;
            tier.UpperRange = null;

            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>();
            price.CataloguePriceTiers.Add(tier);

            solution.CatalogueItem.CataloguePrices.Add(price);

            model.CatalogueItemId = solution.CatalogueItemId;
            model.CataloguePriceId = price.CataloguePriceId;

            service.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage(TieredPriceTiersModelValidator.InvalidStartingRangeError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NoInfiniteRange_SetsModelError(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            TieredPriceTiersModel model,
            [Frozen] Mock<ISolutionListPriceService> service,
            TieredPriceTiersModelValidator validator)
        {
            model.SelectedPublicationStatus = PublicationStatus.Published;

            tier.LowerRange = 1;
            tier.UpperRange = 9;

            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>();
            price.CataloguePriceTiers.Add(tier);

            solution.CatalogueItem.CataloguePrices.Add(price);

            model.CatalogueItemId = solution.CatalogueItemId;
            model.CataloguePriceId = price.CataloguePriceId;

            service.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage(TieredPriceTiersModelValidator.InvalidEndingRangeError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_GapsBetweenRanges_SetsModelError(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier firstTier,
            CataloguePriceTier secondTier,
            TieredPriceTiersModel model,
            [Frozen] Mock<ISolutionListPriceService> service,
            TieredPriceTiersModelValidator validator)
        {
            model.SelectedPublicationStatus = PublicationStatus.Published;

            firstTier.LowerRange = 1;
            firstTier.UpperRange = 9;

            secondTier.LowerRange = 11;
            secondTier.UpperRange = null;

            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>();
            price.CataloguePriceTiers.Add(firstTier);
            price.CataloguePriceTiers.Add(secondTier);

            solution.CatalogueItem.CataloguePrices.Add(price);

            model.CatalogueItemId = solution.CatalogueItemId;
            model.CataloguePriceId = price.CataloguePriceId;

            service.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage(TieredPriceTiersModelValidator.RangeGapError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_OverlapBetweenRanges_SetsModelError(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier firstTier,
            CataloguePriceTier secondTier,
            TieredPriceTiersModel model,
            [Frozen] Mock<ISolutionListPriceService> service,
            TieredPriceTiersModelValidator validator)
        {
            model.SelectedPublicationStatus = PublicationStatus.Published;

            firstTier.LowerRange = 1;
            firstTier.UpperRange = 9;

            secondTier.LowerRange = 8;
            secondTier.UpperRange = null;

            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>();
            price.CataloguePriceTiers.Add(firstTier);
            price.CataloguePriceTiers.Add(secondTier);

            solution.CatalogueItem.CataloguePrices.Add(price);

            model.CatalogueItemId = solution.CatalogueItemId;
            model.CataloguePriceId = price.CataloguePriceId;

            service.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage(TieredPriceTiersModelValidator.RangeOverlapError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier firstTier,
            CataloguePriceTier secondTier,
            TieredPriceTiersModel model,
            [Frozen] Mock<ISolutionListPriceService> service,
            TieredPriceTiersModelValidator validator)
        {
            firstTier.LowerRange = 1;
            firstTier.UpperRange = 9;

            secondTier.LowerRange = 10;
            secondTier.UpperRange = null;

            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>();
            price.CataloguePriceTiers.Add(firstTier);
            price.CataloguePriceTiers.Add(secondTier);

            solution.CatalogueItem.CataloguePrices.Add(price);

            model.CatalogueItemId = solution.CatalogueItemId;
            model.CataloguePriceId = price.CataloguePriceId;

            service.Setup(s => s.GetSolutionWithListPrices(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
