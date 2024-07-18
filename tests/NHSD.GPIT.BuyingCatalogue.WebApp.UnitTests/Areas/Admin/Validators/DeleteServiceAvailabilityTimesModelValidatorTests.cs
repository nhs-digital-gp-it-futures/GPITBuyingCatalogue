using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DeleteServiceAvailabilityTimesModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_PublishedSolutionWithManyServiceAgreementTimes_NoModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            DeleteServiceAvailabilityTimesModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var model = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(1);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PublishedSolutionWitSingleServiceAgreementTimes_SetsModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            DeleteServiceAvailabilityTimesModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var model = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(0);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m)
                .WithErrorMessage("These are the only service availability times provided and can only be deleted if you unpublish your solution first");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_UnpublishedSolutionWithManyServiceAgreementTimes_NoModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            DeleteServiceAvailabilityTimesModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            var model = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(1);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_UnpublishedSolutionWitSingleServiceAgreementTimes_NoModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            DeleteServiceAvailabilityTimesModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            var model = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(solution.CatalogueItemId, serviceAvailabilityTimes.Id).Returns(0);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }
    }
}
