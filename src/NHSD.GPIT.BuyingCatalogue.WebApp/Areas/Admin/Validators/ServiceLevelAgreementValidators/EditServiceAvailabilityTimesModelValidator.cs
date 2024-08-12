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
        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;

        public EditServiceAvailabilityTimesModelValidator(IServiceLevelAgreementsService serviceLevelAgreementsService)
        {
            this.serviceLevelAgreementsService = serviceLevelAgreementsService;

            RuleFor(m => m)
                .Must(NotBeADuplicateServiceAvailabilityTime)
                .WithMessage("Service availability time with these details already exists")
                .OverridePropertyName(
                    m => m.SupportType,
                    m => m.ApplicableDays,
                    m => m.From,
                    m => m.Until);

            RuleFor(m => m.SupportType)
                .NotEmpty()
                .WithMessage("Enter a type of support");

            RuleFor(m => m.From)
                .NotEmpty()
                .WithMessage("Enter a from time");

            RuleFor(m => m.Until)
                .NotEmpty()
                .WithMessage("Enter an until time")
                .Unless(m => !m.From.HasValue);

            RuleFor(m => m.IncludesBankHolidays)
                .NotNull()
                .WithMessage("Select yes if you want to include Bank Holidays");
        }

        private static bool IsTimeEquals(DateTime first, DateTime second)
            => DateTime.Equals(
                CreateDateFromInput(first),
                CreateDateFromInput(second));

        private static DateTime CreateDateFromInput(DateTime input) => new(1970, 1, 1, input.Hour, input.Minute, 0);

        private bool NotBeADuplicateServiceAvailabilityTime(EditServiceAvailabilityTimesModel model)
        {
            var serviceLevelAgreement = serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId).GetAwaiter().GetResult();
            var selectedDays = model.ApplicableDays.Where(y => y.Selected).Select(y => y.Value);

            return !serviceLevelAgreement.ServiceHours.Any(s =>
                s.Id != model.ServiceAvailabilityTimesId
                && string.Equals(s.Category, model.SupportType, StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(s.AdditionalInformation, model.AdditionalInformation, StringComparison.CurrentCultureIgnoreCase)
                && s.IncludedDays.All(x => selectedDays.Contains(x))
                && s.IncludesBankHolidays == model.IncludesBankHolidays
                && IsTimeEquals(s.TimeFrom, model.From.GetValueOrDefault())
                && IsTimeEquals(s.TimeUntil, model.Until.GetValueOrDefault()));
        }
    }
}
