using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels
{
    public sealed class ClientApplicationTypeSelectionModel : ClientApplicationTypeSectionModel
    {
        public ClientApplicationTypeSelectionModel()
        {
        }

        public ClientApplicationTypeSelectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.CatalogueItemId}/client-application-type";
        }

        public bool ApplicationTypesAvailableForSelection { get; set; }

        public ClientApplicationType? SelectedApplicationType { get; set; }
    }
}
