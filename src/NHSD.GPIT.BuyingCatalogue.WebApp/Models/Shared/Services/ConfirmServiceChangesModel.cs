using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services
{
    public class ConfirmServiceChangesModel : NavBaseModel
    {
        public ConfirmServiceChangesModel()
        {
        }

        public ConfirmServiceChangesModel(
            string internalOrgId,
            CatalogueItemType catalogueItemType)
        {
            InternalOrgId = internalOrgId;
            CatalogueItemType = catalogueItemType;
        }

        public string InternalOrgId { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public List<ServiceModel> ToRemove { get; set; }

        public List<ServiceModel> ToAdd { get; set; }

        public bool? ConfirmChanges { get; set; }

        public string EntityType { get; set; } = "Order";

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
