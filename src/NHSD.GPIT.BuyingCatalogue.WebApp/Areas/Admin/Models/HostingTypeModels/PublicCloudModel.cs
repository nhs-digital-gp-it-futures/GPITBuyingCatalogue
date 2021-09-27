using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class PublicCloudModel : BaseCloudModel
    {
        public PublicCloudModel()
        {
        }

        public PublicCloudModel(PublicCloud model)
        {
            Summary = model?.Summary;
            Link = model?.Link;
            RequiresHscn = model?.RequiresHscn;
        }
    }
}
