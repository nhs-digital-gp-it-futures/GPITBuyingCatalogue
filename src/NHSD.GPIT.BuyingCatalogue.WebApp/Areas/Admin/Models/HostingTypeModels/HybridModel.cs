using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class HybridModel : BaseCloudModel
    {
        public HybridModel()
        {
        }

        public HybridModel(CatalogueItem solution)
            : base(solution)
        {
            var hostingType = solution.Solution.Hosting?.HybridHostingType;

            Summary = hostingType?.Summary;
            Link = hostingType?.Link;
            RequiresHscn = hostingType?.RequiresHscn;
            HostingModel = hostingType?.HostingModel;
        }

        [StringLength(1000)]
        public string HostingModel { get; set; }
    }
}
