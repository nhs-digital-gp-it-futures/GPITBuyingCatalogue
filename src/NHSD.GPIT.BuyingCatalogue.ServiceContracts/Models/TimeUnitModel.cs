using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class TimeUnitModel
    {
        [Required(ErrorMessage = "TimeUnitNameRequired")]
        [MaxLength(20, ErrorMessage = "TimeUnitNameTooLong")]
        public string Name { get; set; }

        [Required(ErrorMessage = "TimeUnitDescriptionRequired")]
        [MaxLength(32, ErrorMessage = "TimeUnitDescriptionTooLong")]
        public string Description { get; set; }

        public TimeUnit ToTimeUnit() => TimeUnit.Parse(Name);
    }
}
