using System;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers
{
    public class EditSupplierModelValidator : AbstractValidator<EditSupplierModel>
    {
        public const string MandatorySectionsMissing = "Mandatory section incomplete";

        private const string PublishedSolutionError = "Cannot set to inactive while {0} solutions for this supplier are still published";

        private readonly ISuppliersService suppliersService;

        public EditSupplierModelValidator(ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            RuleFor(m => m.SupplierStatus)
                .Must(HaveCompletedAllMandatorySections)
                .WithMessage(MandatorySectionsMissing)
                .When(m => m.SupplierStatus && m.SupplierStatus != m.CurrentStatus)
                .Must(NotHavePublishedSolutions)
                .WithMessage(PublishedSolutionErrorMessage)
                .When(m => !m.SupplierStatus && m.SupplierStatus != m.CurrentStatus, ApplyConditionTo.CurrentValidator);
        }

        public string PublishedSolutionErrorMessage(EditSupplierModel model, bool supplierStatus)
        {
            _ = supplierStatus;
            var solutions = suppliersService.GetAllSolutionsForSupplier(model.SupplierId).GetAwaiter().GetResult();
            return string.Format(PublishedSolutionError, solutions.Where(s => s.PublishedStatus == PublicationStatus.Published).Count());
        }

        private bool HaveCompletedAllMandatorySections(EditSupplierModel model, bool supplierStatus)
        {
            var supplier = suppliersService.GetSupplier(model.SupplierId).GetAwaiter().GetResult();
            return supplier.Address is not null && supplier.SupplierContacts.Any() && !string.IsNullOrWhiteSpace(supplier.Name) && !string.IsNullOrWhiteSpace(supplier.LegalName);
        }

        private bool NotHavePublishedSolutions(EditSupplierModel model, bool supplierStatus)
        {
            _ = supplierStatus;
            var solutions = suppliersService.GetAllSolutionsForSupplier(model.SupplierId).GetAwaiter().GetResult();
            return !solutions.Any(s => s.PublishedStatus == PublicationStatus.Published);
        }
    }
}
