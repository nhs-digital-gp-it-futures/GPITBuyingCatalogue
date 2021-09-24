using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditListPriceModelValidator : AbstractValidator<EditListPriceModel>
    {
        internal const string TimeUnitErrorMessage = "A time unit must be specified.";
        private readonly ISolutionsService solutionsService;

        public EditListPriceModelValidator(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(p => p.Price)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("A valid price must be entered.")
                .Must(p => Regex.IsMatch(p.ToString(), @"^\d+.?\d{0,4}$"))
                .WithMessage("Price supports up to 4 decimal places.");

            RuleFor(p => p.Unit)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("A unit must be entered.");

            RuleFor(p => p.UnitDefinition)
                .MaximumLength(1000)
                .WithMessage("Unit Definition can only be up to 1,000 characters.")
                .When(p => !string.IsNullOrWhiteSpace(p.UnitDefinition));

            RuleFor(p => p.SelectedProvisioningType)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("A provisioning type must be selected.");

            RuleFor(p => p.DeclarativeTimeUnit)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(TimeUnitErrorMessage)
                .When(p => Equals(p.SelectedProvisioningType, ProvisioningType.Declarative));

            RuleFor(p => p.OnDemandTimeUnit)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(TimeUnitErrorMessage)
                .When(p => Equals(p.SelectedProvisioningType, ProvisioningType.OnDemand));

            RuleFor(p => p)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .MustAsync(NotBeDuplicateOfAnExistingPrice)
                .WithMessage("A list price with these details already exists for this Catalogue Solution.")
                .When(TheModelIsPopulated);
        }

        private static bool TheModelIsPopulated(EditListPriceModel model)
            => model.Unit is not null
               && model.SelectedProvisioningType.HasValue
               && model.GetTimeUnit(model.SelectedProvisioningType.Value) is not null;

        private async Task<bool> NotBeDuplicateOfAnExistingPrice(
            EditListPriceModel model,
            CancellationToken cancellationToken)
        {
            var catalogue = await solutionsService.GetSolution(model.SolutionId);

            return !catalogue.CataloguePrices.Any(cp =>
                  cp.PricingUnit.Description == model.Unit
                  && cp.Price == model.Price
                  && cp.ProvisioningType == model.SelectedProvisioningType.Value
                  && cp.TimeUnit == model.GetTimeUnit(model.SelectedProvisioningType.Value));
        }
    }
}
