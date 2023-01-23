using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum StandardType
    {
        [Description("Overarching")]
        [Display(Name = "Overarching")]
        Overarching = 1,

        [Description("Interoperability")]
        [Display(Name = "Interoperability")]
        Interoperability = 2,

        [Description("Capability")]
        [Display(Name = "Capability")]
        Capability = 3,

        [Description("Context Specific")]
        [Display(Name = "Context Specific")]
        ContextSpecific = 4,
    }
}
