using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class AddSolutionModel : NavBaseModel
    {
        public AddSolutionModel()
        {
            BackLink = "/admin/catalogue-solutions";
            BackLinkText = "Go back";
        }

        [Required(ErrorMessage = "Select a supplier name")]
        public string SupplierId { get; set; }

        [Required(ErrorMessage = "Enter a solution name")]
        [StringLength(255)]
        public string SolutionName { get; set; }

        public string SupplierName { get; set; }

        public IDictionary<string, string> Suppliers { get; set; } = new Dictionary<string, string>();

        public FrameworkModel FrameworkModel { get; set; }
    }

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
                    new[] { string.Empty }
                );
            }
        }
    }
}
