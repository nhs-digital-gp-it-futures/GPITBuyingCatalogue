using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class PrivateCloudModel : BaseCloudModel
    {
        public PrivateCloudModel()
        {
        }

        public PrivateCloudModel(PrivateCloud model)
        {
            Summary = model?.Summary;
            Link = model?.Link;
            RequiresHscn = model?.RequiresHscn;
            HostingModel = model?.HostingModel;
        }

        [Required(ErrorMessage = "Enter data centre model information")]
        [StringLength(1000)]
        public string HostingModel { get; set; }
    }
}
