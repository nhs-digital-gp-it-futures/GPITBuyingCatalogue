using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditListPriceModelValidator : AbstractValidator<EditListPriceModel>
    {
        internal const string TimeUnitErrorMessage = "Select a unit of time";
        private readonly IListPricesService listPriceService;

        public EditListPriceModelValidator(IListPricesService listPriceService)
        {
            this.listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));

            RuleFor(p => p.Price)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Enter a price")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price cannot be negative")
                .Must(p => Regex.IsMatch(p.ToString(), @"^\d+.?\d{0,4}$"))
                .WithMessage("Price must be to a maximum of 4 decimal places");

            RuleFor(p => p.Unit)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Enter a unit");

            RuleFor(p => p.UnitDefinition)
                .MaximumLength(1000)
                .WithMessage("Unit definition must be to a maximum of 1,000 characters")
                .When(p => !string.IsNullOrWhiteSpace(p.UnitDefinition));

            RuleFor(p => p.SelectedProvisioningType)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Select a provisioning type");

            RuleFor(p => p)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .MustAsync(NotBeDuplicateOfAnExistingPrice)
                .WithMessage("A list price with these details already exists for this Catalogue Solution")
                .When(TheModelIsPopulated);
        }

        private static bool TheModelIsPopulated(EditListPriceModel model)
            => model.Unit is not null
               && model.SelectedProvisioningType.HasValue;

        private async Task<bool> NotBeDuplicateOfAnExistingPrice(
            EditListPriceModel model,
            CancellationToken cancellationToken)
        {
            var catalogue = await listPriceService.GetCatalogueItemWithPrices(model.ItemId);

            var duplicatePrices = catalogue!.DuplicateListPrices(
                model.SelectedProvisioningType!.Value,
                model.Price,
                model.Unit,
                model.GetTimeUnit(model.SelectedProvisioningType!.Value));

            if (model.CataloguePriceId is not null)
                duplicatePrices = duplicatePrices.Where(cp => cp.CataloguePriceId != model.CataloguePriceId);

            return !duplicatePrices.Any();
        }
    }
}
