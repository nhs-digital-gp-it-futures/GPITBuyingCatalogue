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
            var hostingType = solution.Solution.Hosting?.PublicCloud;

            Summary = hostingType?.Summary;
            Link = hostingType?.Link;
            RequiresHscn = hostingType?.RequiresHscn;
        }
    }
}
