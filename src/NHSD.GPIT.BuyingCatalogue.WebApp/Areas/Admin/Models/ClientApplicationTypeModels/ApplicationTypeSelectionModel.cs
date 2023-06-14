using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels
{
    public sealed class ApplicationTypeSelectionModel : ApplicationTypeSectionModel
    {
        public ApplicationTypeSelectionModel()
        {
        }

        public ApplicationTypeSelectionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
        }

        public bool ApplicationTypesAvailableForSelection { get; set; }

        public ApplicationType? SelectedApplicationType { get; set; }
    }
}
