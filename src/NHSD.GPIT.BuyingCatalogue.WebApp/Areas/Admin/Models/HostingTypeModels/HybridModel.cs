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
            Summary = solution.Solution.Hosting?.HybridHostingType?.Summary;
            Link = solution.Solution.Hosting?.HybridHostingType?.Link;
            RequiresHscn = solution.Solution.Hosting?.HybridHostingType?.RequiresHscn;
            HostingModel = solution.Solution.Hosting?.HybridHostingType?.HostingModel;
        }

        [Required]
        [StringLength(1000)]
        public string HostingModel { get; set; }
    }
}
