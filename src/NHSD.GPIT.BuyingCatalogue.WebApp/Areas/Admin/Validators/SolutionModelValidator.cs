using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class SolutionModelValidator : AbstractValidator<SolutionModel>
    {
        public SolutionModelValidator()
        {
            RuleFor(s => s.SolutionName)
                .NotEmpty()
                .WithMessage("Enter a solution name");

            RuleFor(s => s.SupplierId)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Select a supplier name");

            RuleFor(s => s.Frameworks)
                .Must(frameworks => frameworks.Any(f => f.Selected))
                .OverridePropertyName($"{nameof(SolutionModel.Frameworks)}[0].Selected")
                .WithMessage("Select the framework(s) your solution is available from");

            When(
                s => s.SupplierId.HasValue && !string.IsNullOrWhiteSpace(s.SolutionName),
                () =>
                {
                    RuleFor(s => s.SolutionName)
                        .Cascade(CascadeMode.Stop)
                        .MaximumLength(255)
                        .WithMessage("Solution name cannot be more than 255 characters");
                });
        }
    }
}
