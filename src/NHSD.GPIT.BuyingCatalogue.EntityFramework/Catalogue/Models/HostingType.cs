using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public enum HostingType
    {
        [Display(Name = "Public cloud")]
        PublicCloud,

        [Display(Name = "Private cloud")]
        PrivateCloud,

        [Display(Name = "Hybrid")]
        Hybrid,

        [Display(Name = "On premise")]
        OnPremise,
    }
}
