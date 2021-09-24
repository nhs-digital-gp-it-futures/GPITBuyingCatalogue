using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class EditSupplierModel
    {
        public string SupplierName { get; set; }

        public string SupplierLegalName { get; set; }

        public string AboutSupplier { get; set; }

        public string SupplierWebsite { get; set; }
    }
}
