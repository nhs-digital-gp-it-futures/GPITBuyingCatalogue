using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DeleteServiceAvailabilityTimesModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_PublishedSolutionWithManyServiceAgreementTimes_NoModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            DeleteServiceAvailabilityTimesModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var model = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetCountOfServiceAvailabilityTimes(serviceAvailabilityTimes.Id))
                .ReturnsAsync(1);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PublishedSolutionWitSingleServiceAgreementTimes_SetsModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            DeleteServiceAvailabilityTimesModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            var model = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetCountOfServiceAvailabilityTimes(serviceAvailabilityTimes.Id))
                .ReturnsAsync(0);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m)
                .WithErrorMessage("These are the only service availability times provided and can only be deleted if you unpublish your solution first");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UnpublishedSolutionWithManyServiceAgreementTimes_NoModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            DeleteServiceAvailabilityTimesModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            var model = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetCountOfServiceAvailabilityTimes(serviceAvailabilityTimes.Id))
                .ReturnsAsync(1);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UnpublishedSolutionWitSingleServiceAgreementTimes_NoModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            DeleteServiceAvailabilityTimesModelValidator validator)
        {
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            var model = new DeleteServiceAvailabilityTimesModel(solution.CatalogueItem, serviceAvailabilityTimes);

            solutionsService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            serviceLevelAgreementsService.Setup(s => s.GetCountOfServiceAvailabilityTimes(serviceAvailabilityTimes.Id))
                .ReturnsAsync(0);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }
    }
}
