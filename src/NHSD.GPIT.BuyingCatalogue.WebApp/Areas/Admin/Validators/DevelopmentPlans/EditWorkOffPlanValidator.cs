using System;
using System.Globalization;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DevelopmentPlans
{
    public sealed class EditWorkOffPlanValidator : AbstractValidator<EditWorkOffPlanModel>
    {
        public const string NoStandardSelectedError = "Select a Standard";
        public const string NoDetailsError = "Enter Work-off Plan item details";
        public const string NoDateDayError = "Agreed completion date must include a day";
        public const string NoDateMonthError = "Agreed completion date must include a month";
        public const string NoDateYearError = "Agreed completion date must include a year";
        public const string DateErrorYearSize = "Year must be four numbers";
        public const string DateIncorrectFormatError = "Enter an agreed completion date in a valid format";
        public const string DuplicateWorkOffPlanError = "A Work-Off Plan with these details already exists";
        public const string DateInPastError = "Agreed completion date must be in the future or within the last 12 weeks";

        private readonly IDevelopmentPlansService developmentPlansService;

        public EditWorkOffPlanValidator(IDevelopmentPlansService developmentPlansService)
        {
            this.developmentPlansService = developmentPlansService
                ?? throw new ArgumentNullException(nameof(developmentPlansService));

            RuleFor(wp => wp.SelectedStandard)
                .NotEmpty()
                .WithMessage(NoStandardSelectedError);

            RuleFor(wp => wp.Details)
                .NotEmpty()
                .WithMessage(NoDetailsError);

            RuleFor(wp => wp.Day)
                .NotEmpty()
                .WithMessage(NoDateDayError);

            RuleFor(wp => wp.Month)
                .NotEmpty()
                .Unless(wp => string.IsNullOrWhiteSpace(wp.Day))
                .OverridePropertyName(wp => wp.Day)
                .WithMessage(NoDateMonthError);

            RuleFor(wp => wp.Year)
                .NotEmpty()
                .Unless(wp => string.IsNullOrWhiteSpace(wp.Month))
                .OverridePropertyName(wp => wp.Day)
                .WithMessage(NoDateYearError)
                .Length(4)
                .Unless(wp => string.IsNullOrWhiteSpace(wp.Month))
                .OverridePropertyName(wp => wp.Day)
                .WithMessage(DateErrorYearSize);

            RuleFor(wp => wp)
                .Must(IsValidDate)
                .Unless(wp =>
                       string.IsNullOrWhiteSpace(wp.Day)
                    || string.IsNullOrWhiteSpace(wp.Month)
                    || string.IsNullOrWhiteSpace(wp.Year))
                .OverridePropertyName(wp => wp.Day)
                .WithMessage(DateIncorrectFormatError);

            RuleFor(wp => wp)
                .Must(NotBeDuplicateWorkOffPlan)
                .Unless(wp => !IsValidDate(wp))
                .OverridePropertyName(wp => wp.Details, wp => wp.SelectedStandard)
                .WithMessage(DuplicateWorkOffPlanError);

            Transform(wp => wp, wp => ParseDate(wp))
                .GreaterThanOrEqualTo(DateTime.Today.AddDays(-84))
                .Unless(wp => !IsValidDate(wp))
                .WithMessage(DateInPastError)
                .OverridePropertyName(wp => wp.Day);
        }

        private static bool IsValidDate(EditWorkOffPlanModel model) =>
            DateTime.TryParseExact($"{model.Day}/{model.Month}/{model.Year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _);

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

        private bool NotBeDuplicateWorkOffPlan(EditWorkOffPlanModel model)
        {
            var workoffPlans = developmentPlansService.GetWorkOffPlans(model.SolutionId).GetAwaiter().GetResult();

            return !workoffPlans.Any(wp =>
            wp.Id != model.WorkOffPlanId
            && string.Equals(wp.Details, model.Details, StringComparison.CurrentCultureIgnoreCase)
            && string.Equals(wp.StandardId, model.SelectedStandard, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
