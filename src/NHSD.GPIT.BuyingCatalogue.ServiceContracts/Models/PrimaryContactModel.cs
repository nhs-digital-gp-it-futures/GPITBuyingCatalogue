using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class PrimaryContactModel
    {
        [Required(ErrorMessage = "First Name Required")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address Required")]
        [EmailAddress]
        [StringLength(256)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Telephone Number Required")]
        [StringLength(35)]
        public string TelephoneNumber { get; set; }
    }
}
