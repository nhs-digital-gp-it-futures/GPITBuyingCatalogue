using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models
{
    public enum OrganisationType
    {
        [Display(Name = "Clinical Commission Group")]
        [EnumMember(Value = "CG")]
        CCG = 1,

        [Display(Name = "Executive Agency")]
        [EnumMember(Value = "EA")]
        EA = 2,

        [Display(Name = "Integrated Care Board")]
        [EnumMember(Value = "IB")]
        IB = 3,
    }
}
