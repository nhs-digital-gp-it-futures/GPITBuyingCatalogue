using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditSolutionContactsModelValidator : AbstractValidator<EditSolutionContactsModel>
    {
        internal static readonly string ErrorElementName = $"AvailableSupplierContacts[0].Selected";

        public EditSolutionContactsModelValidator()
        {
            RuleFor(m => m.AvailableSupplierContacts)
                .Must(HaveAtLeastOneSelectedContact)
                .WithMessage("Select a supplier contact")
                .OverridePropertyName(ErrorElementName)
                .Must(HaveNoMoreThanTwoSelectedContacts)
                .WithMessage("You can only select up to two supplier contacts")
                .OverridePropertyName(ErrorElementName);
        }

        private static bool HaveAtLeastOneSelectedContact(IReadOnlyList<AvailableSupplierContact> availableSupplierContacts)
            => availableSupplierContacts.Any(c => c.Selected);

        private static bool HaveNoMoreThanTwoSelectedContacts(IReadOnlyList<AvailableSupplierContact> availableSupplierContacts)
            => availableSupplierContacts.Count(c => c.Selected) <= 2;
    }
}
