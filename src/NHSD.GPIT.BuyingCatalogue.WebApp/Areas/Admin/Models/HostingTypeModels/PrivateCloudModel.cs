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
            var hostingType = solution.Solution.Hosting?.PrivateCloud;

            Summary = hostingType?.Summary;
            Link = hostingType?.Link;
            RequiresHscn = hostingType?.RequiresHscn;
            HostingModel = hostingType?.HostingModel;
        }

        [Required(ErrorMessage = "Enter data centre model information")]
        [StringLength(1000)]
        public string HostingModel { get; set; }
    }
}
