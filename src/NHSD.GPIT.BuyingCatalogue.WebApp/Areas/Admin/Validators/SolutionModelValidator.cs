using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class SolutionModelValidator : AbstractValidator<SolutionModel>
    {
        public const string SolutionIdRequiredErrorMessage = "Enter a solution ID";
        public const string SolutionIdSupplierMismatchErrorMessage = "The supplier ID does not match the supplier you’ve selected";
        public const string DuplicateSolutionIdErrorMessage = "A solution with that ID already exists. Try a different ID";
        public const string SolutionIdFormatErrorMessage = "Solution ID must be in the correct format, for example 10000-001";

        private readonly ISolutionsService solutionsService;

        public SolutionModelValidator(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(s => s.SolutionName)
                .NotEmpty()
                .WithMessage("Enter a solution name");

            RuleFor(s => s)
                .Must(NotBeADuplicateName)
                .WithMessage("A solution with this name already exists")
                .OverridePropertyName(m => m.SolutionName);

            RuleFor(s => s.SupplierId)
                .NotEmpty()
                .WithMessage("Select a supplier");

            RuleFor(s => s.SolutionIdDisplay)
                .NotNull()
                .WithMessage(SolutionIdRequiredErrorMessage);

            RuleFor(s => s.SolutionId)
                .NotNull()
                .WithMessage(SolutionIdFormatErrorMessage)
                .Must((model, id) => model.SupplierId == id.GetValueOrDefault().SupplierId)
                .WithMessage(SolutionIdSupplierMismatchErrorMessage)
                .Must(NotBeADuplicateSolutionId)
                .WithMessage(DuplicateSolutionIdErrorMessage)
                .When(m => !m.IsEdit && !string.IsNullOrWhiteSpace(m.SolutionIdDisplay))
                .OverridePropertyName(m => m.SolutionIdDisplay);

            RuleFor(s => s.Frameworks)
                .Must(frameworks => frameworks.Any(f => f.Selected))
                .OverridePropertyName($"{nameof(SolutionModel.Frameworks)}[0].Selected")
                .WithMessage("Select the framework(s) your solution is available from");

            RuleFor(s => s.SolutionName)
                .MaximumLength(255)
                .WithMessage("Solution name cannot be more than 255 characters")
                .When(s => s.SupplierId.HasValue && !string.IsNullOrWhiteSpace(s.SolutionName));

            RuleFor(s => s.Frameworks)
                .Must(framework => framework.Count(x => x.IsFoundation && x.Selected) <= 1)
                .OverridePropertyName($"{nameof(SolutionModel.Frameworks)}[0].Selected")
                .WithMessage("A solution can only be marked as a Foundation Solution under one Framework at any given time");
        }

        private bool NotBeADuplicateSolutionId(CatalogueItemId? id)
        {
            return solutionsService.GetSolution(id.GetValueOrDefault()).GetAwaiter().GetResult() is null;
        }

        private bool NotBeADuplicateName(SolutionModel model)
        {
            return !(model.IsEdit
                ? solutionsService.CatalogueSolutionExistsWithName(model.SolutionName, model.SolutionId.GetValueOrDefault()).GetAwaiter().GetResult()
                : solutionsService.CatalogueSolutionExistsWithName(model.SolutionName).GetAwaiter().GetResult());
        }
    }
}
