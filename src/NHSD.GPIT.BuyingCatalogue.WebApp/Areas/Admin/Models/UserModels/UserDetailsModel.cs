using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class UserDetailsModel : NavBaseModel
    {
        private string firstName;
        private string lastName;
        private string email;

        public UserDetailsModel()
        {
        }

        public UserDetailsModel(AspNetUser user)
        {
            SelectedOrganisationId = user.PrimaryOrganisationId;
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            SelectedAccountType = user.GetRoleName();
            IsActive = !user.Disabled;
        }

        public override string Title
        {
            get
            {
                return UserId == 0 ? "Add user" : "Edit user";
            }
        }

        public int UserId { get; set; }

        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value?.Trim();
            }
        }

        [StringLength(100)]
        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value?.Trim();
            }
        }

        [StringLength(256)]
        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value?.Trim();
            }
        }

        public string SelectedAccountType { get; set; }

        public int? SelectedOrganisationId { get; set; }

        public IEnumerable<SelectOption<string>> AccountTypeOptions => new List<SelectOption<string>>
        {
            new(OrganisationFunction.Buyer.DisplayName, OrganisationFunction.Buyer.InternalAdvice, $"{OrganisationFunction.Buyer.Name}"),
            new(OrganisationFunction.AccountManager.DisplayName, OrganisationFunction.AccountManager.InternalAdvice, $"{OrganisationFunction.AccountManager.Name}"),
            new(OrganisationFunction.Authority.DisplayName, OrganisationFunction.Authority.InternalAdvice, $"{OrganisationFunction.Authority.Name}"),
        };

        public bool? IsActive { get; set; }

        public IEnumerable<SelectOption<bool>> StatusOptions => new List<SelectOption<bool>>
        {
            new("Active", true),
            new("Inactive", false),
        };

        public IEnumerable<SelectOption<string>> Organisations { get; set; }
    }
}
