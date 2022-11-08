using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
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

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string EmailAddress { get; set; }

        public string ControllerName { get; set; }
    }
}
