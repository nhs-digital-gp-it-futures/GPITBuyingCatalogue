using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class PublicCloudModel : BaseCloudModel
    {
        public PublicCloudModel()
            : base()
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
