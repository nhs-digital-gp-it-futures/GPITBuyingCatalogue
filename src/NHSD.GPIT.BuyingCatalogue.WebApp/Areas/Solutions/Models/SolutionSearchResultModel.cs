using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionSearchResultModel
    {
        public SolutionSearchResultModel(CatalogueItem catalogueItem, ICollection<CapabilitiesAndCountModel> selectedCapabilityIds, bool noLinks = false)
        {
            CatalogueItem = catalogueItem;
            SelectedCapabilityIds = selectedCapabilityIds;
            NoLinks = noLinks;
        }

        public CatalogueItem CatalogueItem { get; init; }

        public ICollection<CapabilitiesAndCountModel> SelectedCapabilityIds { get; init; }

        public bool NoLinks { get; init; }
    }
}
