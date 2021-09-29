using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class PublicCloudModel : BaseCloudModel
    {
        public PublicCloudModel()
        {
        }

        public PublicCloudModel(CatalogueItem solution)
            : base(solution)
        {
            Summary = solution.Solution.Hosting?.PublicCloud?.Summary;
            Link = solution.Solution.Hosting?.PublicCloud?.Link;
            RequiresHscn = solution.Solution.Hosting?.PublicCloud?.RequiresHscn;
        }
    }
}
