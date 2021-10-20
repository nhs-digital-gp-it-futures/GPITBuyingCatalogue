using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditAdditionalServiceDetailsModelValidator : AbstractValidator<EditAdditionalServiceDetailsModel>
    {
        private readonly ISuppliersService suppliersService;

        public EditAdditionalServiceDetailsModelValidator(ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            RuleFor(m => m)
                .MustAsync(NotBeADuplicateService)
                .WithMessage("Additional Service name already exists. Enter a different name")
                .OverridePropertyName(m => m.Name);

            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage("Enter an Additional Service name");

            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage("Enter an Additional Service description");
        }

        private async Task<bool> NotBeADuplicateService(EditAdditionalServiceDetailsModel model, CancellationToken cancellationToken)
        {
            var allSolutions = await suppliersService.GetAllSolutionsForSupplier(model.SupplierId);

            if (model.Id is not null)
                allSolutions = allSolutions.Where(s => s.Id != model.Id).ToList();

            return !allSolutions.Any(s => string.Equals(s.Name, model.Name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
