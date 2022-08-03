using System;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public sealed class EditServiceAvailabilityTimesModelValidator : AbstractValidator<EditServiceAvailabilityTimesModel>
    {
        public const string ServiceAvailabilityTimeDuplicateError = "Service availability time with these details already exists";
        public const string NoTypeOfSupportError = "Enter a type of support";
        public const string NoFromTimeError = "Enter a from time";
        public const string NoUntilTimeError = "Enter an until time";
        public const string NoApplicableDaysError = "Enter the applicable days";

        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;

        public EditServiceAvailabilityTimesModelValidator(IServiceLevelAgreementsService serviceLevelAgreementsService)
        {
            this.serviceLevelAgreementsService = serviceLevelAgreementsService;

            RuleFor(m => m)
                .Must(NotBeADuplicateServiceAvailabilityTime)
                .WithMessage(ServiceAvailabilityTimeDuplicateError)
                .OverridePropertyName(
                    m => m.SupportType,
                    m => m.ApplicableDays,
                    m => m.From,
                    m => m.Until);

            RuleFor(m => m.SupportType)
                .NotEmpty()
                .WithMessage(NoTypeOfSupportError);

            RuleFor(m => m.From)
                .NotEmpty()
                .WithMessage(NoFromTimeError);

            RuleFor(m => m.Until)
                .NotEmpty()
                .WithMessage(NoUntilTimeError)
                .Unless(m => !m.From.HasValue);

            RuleFor(m => m.ApplicableDays)
                .NotEmpty()
                .WithMessage(NoApplicableDaysError);
        }

        private static bool IsTimeEquals(DateTime first, DateTime second)
            => DateTime.Equals(
                CreateDateFromInput(first),
                CreateDateFromInput(second));

        private static DateTime CreateDateFromInput(DateTime input) => new(1970, 1, 1, input.Hour, input.Minute, 0);

        private bool NotBeADuplicateServiceAvailabilityTime(EditServiceAvailabilityTimesModel model)
        {
            var serviceLevelAgreement = serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId).GetAwaiter().GetResult();

            return !serviceLevelAgreement.ServiceHours.Any(s =>
                s.Id != model.ServiceAvailabilityTimesId
                && string.Equals(s.Category, model.SupportType, StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(s.ApplicableDays, model.ApplicableDays, StringComparison.CurrentCultureIgnoreCase)
                && IsTimeEquals(s.TimeFrom, model.From.GetValueOrDefault())
                && IsTimeEquals(s.TimeUntil, model.Until.GetValueOrDefault()));
        }
    }
}
