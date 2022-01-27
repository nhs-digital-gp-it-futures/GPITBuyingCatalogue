using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics
{
    public sealed class EditSupplierDefinedEpicModelValidator : AbstractValidator<EditSupplierDefinedEpicModel>
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public EditSupplierDefinedEpicModelValidator(
            ISupplierDefinedEpicsService supplierDefinedEpicsService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService;

            Include(new SupplierDefinedEpicBaseModelValidator(supplierDefinedEpicsService));

            RuleFor(m => m.IsActive)
                .MustAsync(NotBeReferencedByAnySolutions)
                .WithMessage("This supplier defined Epic cannot be set to inactive as it is referenced by another solution or service")
                .When(m => m.IsActive == false);
        }

        public async Task<bool> NotBeReferencedByAnySolutions(EditSupplierDefinedEpicModel model, bool? isActive, CancellationToken token)
        {
            _ = token;

            var epic = await supplierDefinedEpicsService.GetEpic(model.Id);
            if (epic.IsActive == isActive)
                return true;

            var itemsReferencingEpic = await supplierDefinedEpicsService.GetItemsReferencingEpic(model.Id);
            return itemsReferencingEpic.Count == 0;
        }
    }
}
