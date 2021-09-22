using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum ProvisioningType
    {
        [Display(Name = "Patient")]
        Patient = 1,

        [Display(Name = "Declarative")]
        Declarative = 2,

        [Display(Name = "On demand")]
        OnDemand = 3,
    }
}
