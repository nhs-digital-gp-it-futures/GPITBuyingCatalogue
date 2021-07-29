using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class HostingTypeSelectionModel : HostingTypeSectionModel
    {
        public HostingTypeSelectionModel()
        {
        }

        public HostingTypeSelectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.CatalogueItemId}/hosting-type";
        }

        public bool HostingTypesAvailableForSelection { get; set; }

        public HostingType? SelectedHostingType { get; set; }
    }
}
