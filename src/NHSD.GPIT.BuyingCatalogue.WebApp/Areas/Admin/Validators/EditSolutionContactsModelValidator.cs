using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditSolutionContactsModelValidator : AbstractValidator<EditSolutionContactsModel>
    {
        public const string ErrorElementName = $"AvailableSupplierContacts[0].Selected";
        public const string NoSelectedContactError = "Select a supplier contact";
        public const string MoreThanTwoContactsError = "You can only select up to two supplier contacts";

        public EditSolutionContactsModelValidator()
        {
            RuleFor(m => m.AvailableSupplierContacts)
                .Must(HaveAtLeastOneSelectedContact)
                .WithMessage(NoSelectedContactError)
                .OverridePropertyName(ErrorElementName)
                .Must(HaveNoMoreThanTwoSelectedContacts)
                .WithMessage(MoreThanTwoContactsError)
                .OverridePropertyName(ErrorElementName);
        }

        private static bool HaveAtLeastOneSelectedContact(IReadOnlyList<AvailableSupplierContact> availableSupplierContacts)
            => availableSupplierContacts.Any(c => c.Selected);

        private static bool HaveNoMoreThanTwoSelectedContacts(IReadOnlyList<AvailableSupplierContact> availableSupplierContacts)
            => availableSupplierContacts.Count(c => c.Selected) <= 2;
    }
}
