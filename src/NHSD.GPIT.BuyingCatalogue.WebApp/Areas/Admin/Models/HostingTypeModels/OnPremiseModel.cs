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
            Summary = solution.Solution.Hosting?.OnPremise?.Summary;
            Link = solution.Solution.Hosting?.OnPremise?.Link;
            RequiresHscn = solution.Solution.Hosting?.OnPremise?.RequiresHscn;
            HostingModel = solution.Solution.Hosting?.OnPremise?.HostingModel;
        }

        [Required]
        [StringLength(1000)]
        public string HostingModel { get; set; }
    }
}
