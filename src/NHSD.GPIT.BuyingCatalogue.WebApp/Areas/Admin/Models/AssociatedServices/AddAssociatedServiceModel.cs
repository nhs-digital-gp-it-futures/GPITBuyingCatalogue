using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class AddAssociatedServiceModel : NavBaseModel
    {
        public AddAssociatedServiceModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/associated-services";
            BackLinkText = "Go back";
            Solution = catalogueItem;
        }

        public CatalogueItem Solution { get; }
    }
}
