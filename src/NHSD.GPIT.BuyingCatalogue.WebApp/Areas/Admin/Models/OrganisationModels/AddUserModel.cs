using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public sealed class AddUserModel : NavBaseModel
    {
        public AddUserModel()
        {
        }

        public AddUserModel(Organisation organisation)
        {
            OrganisationName = organisation.Name;
        }

        public string OrganisationName { get; set; }

        [Required(ErrorMessage = "First Name Required")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Telephone Number Required")]
        [StringLength(35)]
        public string TelephoneNumber { get; set; }

        [Required(ErrorMessage = "Email Address Required")]
        [StringLength(256)]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
