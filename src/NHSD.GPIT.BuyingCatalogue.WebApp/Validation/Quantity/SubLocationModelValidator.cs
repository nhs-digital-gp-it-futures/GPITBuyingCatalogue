using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity
{
    [ChildValidator]
    public class SubLocationModelValidator : AbstractValidator<SubLocationModel>
    {
        public SubLocationModelValidator(
            ProvisioningType provisioningType,
            CatalogueItemType catalogueItemType)
        {
            RuleForEach(x => x.ServiceRecipients)
                .Cascade(CascadeMode.Continue)
                .SetValidator(new ServiceRecipientQuantityModelValidator(provisioningType, catalogueItemType));
        }
    }
}
