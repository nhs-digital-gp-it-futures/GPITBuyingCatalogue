using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class OnPremiseModel : BaseCloudModel
    {
        public OnPremiseModel()
        {
        }

        public OnPremiseModel(CatalogueItem solution)
            : base(solution)
        {
            var hostingType = solution.Solution.Hosting?.OnPremise;

            Summary = hostingType?.Summary;
            Link = hostingType?.Link;
            RequiresHscn = hostingType?.RequiresHscn;
            HostingModel = hostingType?.HostingModel;
        }

        [Required]
        [StringLength(1000)]
        public string HostingModel { get; set; }
    }
}
