using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Shared
{
    public class ConfirmServiceChangesModel : NavBaseModel
    {
        public ConfirmServiceChangesModel()
        {
        }

        public ConfirmServiceChangesModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemType catalogueItemType)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CatalogueItemType = catalogueItemType;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public List<ServiceModel> ToRemove { get; set; }

        public List<ServiceModel> ToAdd { get; set; }

        public bool? ConfirmChanges { get; set; }

        public IEnumerable<SelectOption<string>> SelectListItems => new[]
        {
            new SelectOption<string>($"Yes, I want to confirm changes to my {ItemDescription}", $"{true}"),
            new SelectOption<string>($"No, I want to keep my current {ItemDescription}", $"{false}"),
        };

        private string ItemDescription => CatalogueItemType == CatalogueItemType.Solution
            ? CatalogueItemType.Name()
            : $"{CatalogueItemType.Name()}s";
    }
}
