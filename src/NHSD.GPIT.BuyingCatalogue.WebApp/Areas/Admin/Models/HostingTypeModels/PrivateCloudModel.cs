using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class PrivateCloudModel : BaseCloudModel
    {
        public PrivateCloudModel()
        {
        }

        public PrivateCloudModel(CatalogueItem solution)
            : base(solution)
        {
            Summary = solution.Solution.Hosting?.PrivateCloud?.Summary;
            Link = solution.Solution.Hosting?.PrivateCloud?.Link;
            RequiresHscn = solution.Solution.Hosting?.PrivateCloud?.RequiresHscn;
            HostingModel = solution.Solution.Hosting?.PrivateCloud?.HostingModel;
        }

        [Required(ErrorMessage = "Enter data centre model information")]
        [StringLength(1000)]
        public string HostingModel { get; set; }
    }
}
