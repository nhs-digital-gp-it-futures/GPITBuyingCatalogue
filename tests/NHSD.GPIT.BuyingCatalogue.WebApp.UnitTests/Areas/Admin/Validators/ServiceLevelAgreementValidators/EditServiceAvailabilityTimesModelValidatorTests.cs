using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public static class EditServiceAvailabilityTimesModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_SupportTypeNotEntered_SetsModelError(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var model = new EditServiceAvailabilityTimesModel
            {
                From = DateTime.UtcNow, Until = DateTime.UtcNow.AddMinutes(5),
            };

            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupportType)
                .WithErrorMessage("Enter a type of support");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_FromNotEntered_SetsModelError(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var model = new EditServiceAvailabilityTimesModel
            {
                SupportType = "Service hours", Until = DateTime.UtcNow,
            };

            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.From)
                .WithErrorMessage("Enter a from time");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_UntilNotEntered_SetsModelError(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var model = new EditServiceAvailabilityTimesModel
            {
                SupportType = "Service hours", From = DateTime.UtcNow,
            };

            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Until)
                .WithErrorMessage("Enter an until time");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_BankHolidaysNotEntered_SetsModelError(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var model = new EditServiceAvailabilityTimesModel
            {
                SupportType = "Service hours",
                From = DateTime.UtcNow,
                Until = DateTime.UtcNow.AddHours(3),
                IncludesBankHolidays = null,
            };

            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IncludesBankHolidays)
                .WithErrorMessage("Select yes if you want to include Bank Holidays");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AddDuplicateAvailabilityTimes_SetsModelError(
            Solution solution,
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var serviceLevelAgreements = new ServiceLevelAgreements
            {
                SolutionId = solution.CatalogueItemId,
                ServiceHours = new HashSet<ServiceAvailabilityTimes> { serviceAvailabilityTimes, },
            };

            serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(serviceAvailabilityTimes.SolutionId)
                .Returns(serviceLevelAgreements);

            var model = new EditServiceAvailabilityTimesModel(solution.CatalogueItem)
            {
                ServiceAvailabilityTimesId = serviceAvailabilityTimes.Id + 1,
                SolutionId = serviceAvailabilityTimes.SolutionId,
                SupportType = serviceAvailabilityTimes.Category,
                From = serviceAvailabilityTimes.TimeFrom,
                Until = serviceAvailabilityTimes.TimeUntil,
                IncludesBankHolidays = serviceAvailabilityTimes.IncludesBankHolidays,
                AdditionalInformation = serviceAvailabilityTimes.AdditionalInformation,
            };

            model.ApplicableDays.ForEach(x => x.Selected = serviceAvailabilityTimes.IncludedDays.Contains(x.Value));

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError()
                .WithErrorMessage("Service availability time with these details already exists");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EditExistingServiceAvailabilityTime_NoModelError(
            ServiceAvailabilityTimes serviceAvailabilityTimes,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            EditServiceAvailabilityTimesModelValidator validator)
        {
            var serviceLevelAgreements = new ServiceLevelAgreements
            {
                SolutionId = serviceAvailabilityTimes.SolutionId,
                ServiceHours = new HashSet<ServiceAvailabilityTimes> { serviceAvailabilityTimes, },
            };

            serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(serviceAvailabilityTimes.SolutionId)
                .Returns(serviceLevelAgreements);

            var model = new EditServiceAvailabilityTimesModel
            {
                ServiceAvailabilityTimesId = serviceAvailabilityTimes.Id,
                SolutionId = serviceAvailabilityTimes.SolutionId,
                SupportType = serviceAvailabilityTimes.Category,
                From = serviceAvailabilityTimes.TimeFrom,
                Until = serviceAvailabilityTimes.TimeUntil,
            };

            model.ApplicableDays.ForEach(x => x.Selected = serviceAvailabilityTimes.IncludedDays.Contains(x.Value));

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }
    }
}
