using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DevelopmentPlans
{
    public sealed class EditWorkOffPlanValidator : AbstractValidator<EditWorkOffPlanModel>
    {
        private readonly IDevelopmentPlansService developmentPlansService;

        public EditWorkOffPlanValidator(IDevelopmentPlansService developmentPlansService)
        {
            this.developmentPlansService = developmentPlansService
                ?? throw new ArgumentNullException(nameof(developmentPlansService));

            RuleFor(wp => wp.SelectedStandard)
                .NotEmpty()
                .WithMessage("Select a Standard");

            RuleFor(wp => wp.Details)
                .NotEmpty()
                .WithMessage("Enter Work-off Plan item details");

            RuleFor(wp => wp.Day)
                .NotEmpty()
                .WithMessage("Agreed completion date must include a day");

            RuleFor(wp => wp.Month)
                .NotEmpty()
                .Unless(wp => string.IsNullOrWhiteSpace(wp.Day))
                .OverridePropertyName(wp => wp.Day)
                .WithMessage("Agreed completion date must include a month");

            RuleFor(wp => wp.Year)
                .NotEmpty()
                .Unless(wp => string.IsNullOrWhiteSpace(wp.Month))
                .OverridePropertyName(wp => wp.Day)
                .WithMessage("Agreed completion date must include a year");

            RuleFor(wp => wp.Year)
                .Length(4)
                .OverridePropertyName(wp => wp.Day)
                .WithMessage("Year must be four numbers");

            RuleFor(wp => wp)
                .Must(IsValidDate)
                .Unless(wp =>
                       string.IsNullOrWhiteSpace(wp.Day)
                    || string.IsNullOrWhiteSpace(wp.Month)
                    || string.IsNullOrWhiteSpace(wp.Year))
                .OverridePropertyName(wp => wp.Day)
                .WithMessage("Enter an agreed completion date in a valid format");

            RuleFor(wp => wp)
                .MustAsync(NotBeDuplicateWorkOffPlan)
                .Unless(wp => !IsValidDate(wp))
                .OverridePropertyName(wp => wp.Details, wp => wp.SelectedStandard)
                .WithMessage("A Work-Off Plan with these details already exists");

            Transform(wp => wp, wp => ParseDate(wp))
                .GreaterThanOrEqualTo(DateTime.Today)
                .Unless(wp => !IsValidDate(wp))
                .WithMessage("Agreed completion date must be in the future")
                .OverridePropertyName(wp => wp.Day);
        }

        private static bool IsValidDate(EditWorkOffPlanModel model) =>
            DateTime.TryParseExact($"{model.Day}/{model.Month}/{model.Year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

        private static DateTime ParseDate(EditWorkOffPlanModel model)
        {
            if (DateTime.TryParseExact(
                $"{model.Day}/{model.Month}/{model.Year}",
                "d/M/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;
        }

        private async Task<bool> NotBeDuplicateWorkOffPlan(EditWorkOffPlanModel model, CancellationToken cancellationToken)
        {
            _ = cancellationToken;

            var workoffPlans = await developmentPlansService.GetWorkOffPlans(model.SolutionId);

            return !workoffPlans.Any(wp =>
            wp.Id != model.WorkOffPlanId
            && string.Equals(wp.Details, model.Details, StringComparison.CurrentCultureIgnoreCase)
            && string.Equals(wp.StandardId, model.SelectedStandard, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
