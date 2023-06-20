using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels
{
    public sealed class ClientApplicationTypeSelectionModel : ApplicationTypeSectionModel
    {
        public ClientApplicationTypeSelectionModel()
        {
        }

        public ClientApplicationTypeSelectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
        }

        public bool ApplicationTypesAvailableForSelection { get; set; }

        public ApplicationType? SelectedApplicationType { get; set; }
    }
}
