using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class FrameworkModel
    {
        public bool DfocvcFramework { get; set; }

        public bool FoundationSolutionFramework { get; set; }

        public bool GpitFuturesFramework { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!DfocvcFramework && !GpitFuturesFramework)
            {
                yield return new ValidationResult(
                    "Select the framework(s) your solution is available from",
                    new[] { string.Empty });
            }
        }

        public virtual bool IsValid() => DfocvcFramework || GpitFuturesFramework;
    }
}
