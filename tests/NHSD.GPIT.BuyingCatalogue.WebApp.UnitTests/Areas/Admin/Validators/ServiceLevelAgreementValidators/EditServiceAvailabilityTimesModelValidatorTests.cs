using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public static class EditServiceAvailabilityTimesModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SupportTypeNotEntered_SetsModelError(
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var model = new EditServiceAvailabilityTimesModel
            {
                ApplicableDays = "Monday",
                From = DateTime.UtcNow,
                Until = DateTime.UtcNow.AddMinutes(5),
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupportType)
                .WithErrorMessage("Enter a type of support");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_FromNotEntered_SetsModelError(
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var model = new EditServiceAvailabilityTimesModel
            {
                SupportType = "Service hours",
                ApplicableDays = "Monday",
                Until = DateTime.UtcNow,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.From)
                .WithErrorMessage("Enter a from time");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UntilNotEntered_SetsModelError(
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var model = new EditServiceAvailabilityTimesModel
            {
                SupportType = "Service hours",
                ApplicableDays = "Monday",
                From = DateTime.UtcNow,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Until)
                .WithErrorMessage("Enter an until time");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ApplicableDAysNotEntered_SetsModelError(
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var model = new EditServiceAvailabilityTimesModel
            {
                SupportType = "Serivce hours",
                From = DateTime.UtcNow,
                Until = DateTime.UtcNow.AddMinutes(5),
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApplicableDays)
                .WithErrorMessage("Enter the applicable days");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AddDuplicateAvailabilityTimes_SetsModelError(
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var serviceLevelAgreements = new ServiceLevelAgreements
            {
                SolutionId = serviceAvailabilityTimes.SolutionId,
                ServiceHours = new HashSet<ServiceAvailabilityTimes>
                {
                    serviceAvailabilityTimes,
                },
            };

            serviceLevelAgreementsService.Setup(s => s.GetServiceLevelAgreementForSolution(serviceAvailabilityTimes.SolutionId))
                .ReturnsAsync(serviceLevelAgreements);

            var model = new EditServiceAvailabilityTimesModel
            {
                ServiceAvailabilityTimesId = serviceAvailabilityTimes.Id + 1,
                SolutionId = serviceAvailabilityTimes.SolutionId,
                SupportType = serviceAvailabilityTimes.Category,
                From = serviceAvailabilityTimes.TimeFrom,
                Until = serviceAvailabilityTimes.TimeUntil,
                ApplicableDays = serviceAvailabilityTimes.ApplicableDays,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Service availability time with these details already exists");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EditExistingServiceAvailabilityTime_NoModelError(
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var serviceLevelAgreements = new ServiceLevelAgreements
            {
                SolutionId = serviceAvailabilityTimes.SolutionId,
                ServiceHours = new HashSet<ServiceAvailabilityTimes>
                {
                    serviceAvailabilityTimes,
                },
            };

            serviceLevelAgreementsService.Setup(s => s.GetServiceLevelAgreementForSolution(serviceAvailabilityTimes.SolutionId))
                .ReturnsAsync(serviceLevelAgreements);

            var model = new EditServiceAvailabilityTimesModel
            {
                ServiceAvailabilityTimesId = serviceAvailabilityTimes.Id,
                SolutionId = serviceAvailabilityTimes.SolutionId,
                SupportType = serviceAvailabilityTimes.Category,
                From = serviceAvailabilityTimes.TimeFrom,
                Until = serviceAvailabilityTimes.TimeUntil,
                ApplicableDays = serviceAvailabilityTimes.ApplicableDays,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }
    }
}
