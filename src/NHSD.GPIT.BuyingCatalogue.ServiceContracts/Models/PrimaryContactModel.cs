using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class PrimaryContactModel
    {
        public int? SupplierContactId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string EmailAddress { get; set; }

        [StringLength(35)]
        public string TelephoneNumber { get; set; }

        [StringLength(50)]
        public string Department { get; set; }
    }
}
