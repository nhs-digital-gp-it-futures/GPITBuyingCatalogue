using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum ProvisioningType
    {
        [Display(Name = "Per patient per year")]
        Patient = 1,

        [Display(Name = "Declarative")]
        Declarative = 2,

        [Display(Name = "On demand")]
        OnDemand = 3,

        [Obsolete("This was removed from the UI as part of story `(21663) Disable the Per Service Recipient provisioning type` and is not used in production. see PR #990 and Commit #7923ae. We should look to remove code that uses this.")]
        [Display(Name = "Per Service Recipient")]
        PerServiceRecipient = 4,
    }
}
