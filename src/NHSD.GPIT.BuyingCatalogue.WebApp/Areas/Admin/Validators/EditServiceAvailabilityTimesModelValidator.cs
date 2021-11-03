using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditServiceAvailabilityTimesModelValidator : AbstractValidator<EditServiceAvailabilityTimesModel>
    {
        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;

        public EditServiceAvailabilityTimesModelValidator(IServiceLevelAgreementsService serviceLevelAgreementsService)
        {
            this.serviceLevelAgreementsService = serviceLevelAgreementsService;

            RuleFor(m => m)
                .MustAsync(NotBeADuplicateServiceAvailabilityTime)
                .WithMessage("Service availability time with these details already exists");

            RuleFor(m => m.SupportType)
                .NotEmpty()
                .WithMessage("Enter a type of support");

            RuleFor(m => m.From)
                .NotNull()
                .WithMessage("Enter a from time");

            RuleFor(m => m.Until)
                .NotNull()
                .WithMessage("Enter an until time");

            RuleFor(m => m.ApplicableDays)
                .NotEmpty()
                .WithMessage("Enter the applicable days");
        }

        private static bool IsTimeEquals(DateTime first, DateTime second)
            => DateTime.Equals(
                CreateDateFromInput(first),
                CreateDateFromInput(second));

        private static DateTime CreateDateFromInput(DateTime input) => new(1970, 1, 1, input.Hour, input.Minute, 0);

        private async Task<bool> NotBeADuplicateServiceAvailabilityTime(EditServiceAvailabilityTimesModel model, CancellationToken cancellationToken)
        {
            var serviceLevelAgreement = await serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId);

            return !serviceLevelAgreement.ServiceHours.Any(s =>
                s.Id != model.ServiceAvailabilityTimesId
                && string.Equals(s.Category, model.SupportType, StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(s.ApplicableDays, model.ApplicableDays, StringComparison.CurrentCultureIgnoreCase)
                && IsTimeEquals(s.TimeFrom, model.From.GetValueOrDefault())
                && IsTimeEquals(s.TimeUntil, model.Until.GetValueOrDefault()));
        }
    }
}
