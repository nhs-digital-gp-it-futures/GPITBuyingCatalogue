using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public enum HostingType
    {
        [Display(Name = "Public cloud")]
        PublicCloud = 1,

        [Display(Name = "Private cloud")]
        PrivateCloud = 2,

        [Display(Name = "Hybrid")]
        Hybrid = 3,

        [Display(Name = "On premise")]
        OnPremise = 4,
    }
}
