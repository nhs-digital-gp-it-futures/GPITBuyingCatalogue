using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class HybridModel : BaseCloudModel
    {
        public HybridModel()
            : base()
        {
        }

        public HybridModel(HybridHostingType model)
        {
            Summary = model?.Summary;
            Link = model?.Link;
            RequiresHscn = model?.RequiresHscn;
            HostingModel = model?.HostingModel;
        }

        [Required]
        [StringLength(1000)]
        public string HostingModel { get; set; }

        public override bool? IsValid() =>
            !string.IsNullOrWhiteSpace(Summary) ||
            !string.IsNullOrWhiteSpace(Link) ||
            !string.IsNullOrWhiteSpace(RequiresHscn) ||
            !string.IsNullOrWhiteSpace(HostingModel);
    }
}
