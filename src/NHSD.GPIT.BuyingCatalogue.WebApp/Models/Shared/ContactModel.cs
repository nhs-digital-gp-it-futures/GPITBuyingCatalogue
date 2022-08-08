using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared
{
    public abstract class ContactModel : NavBaseModel
    {
        [StringLength(35)]
        public string FirstName { get; set; }

        [StringLength(35)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        [StringLength(35)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Email { get; set; }
    }
}
