using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics
{
    public sealed class DeleteSupplierDefinedEpicConfirmationModelValidator
        : AbstractValidator<DeleteSupplierDefinedEpicConfirmationModel>
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public DeleteSupplierDefinedEpicConfirmationModelValidator(
            ISupplierDefinedEpicsService supplierDefinedEpicsService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService;

            RuleFor(m => m)
                .MustAsync(NotBeActiveOrReferenced);
        }

        private async Task<bool> NotBeActiveOrReferenced(DeleteSupplierDefinedEpicConfirmationModel model, CancellationToken cancellationToken)
        {
            _ = cancellationToken;

            var epic = await supplierDefinedEpicsService.GetEpic(model.Id);
            var referencingItems = await supplierDefinedEpicsService.GetItemsReferencingEpic(model.Id);

            return !epic.IsActive && referencingItems.Count == 0;
        }
    }
}
