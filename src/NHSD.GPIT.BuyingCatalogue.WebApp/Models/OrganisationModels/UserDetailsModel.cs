using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public sealed class UserDetailsModel : NavBaseModel
    {
        private bool isDefaultAccountType;

        public UserDetailsModel()
        {
        }

        public UserDetailsModel(Organisation organisation, AspNetUser user)
            : this(organisation)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAddress = user.Email;
            SelectedAccountType = user.GetRoleName();
            IsActive = !user.Disabled;
        }

        public UserDetailsModel(Organisation organisation)
        {
            OrganisationId = organisation.Id;
            OrganisationName = organisation.Name;
        }

        public string Title
        {
            get
            {
                return UserId == 0 ? "Add user" : "Edit user";
            }
        }

        public int OrganisationId { get; set; }

        public int UserId { get; set; }

        public int MaxNumberOfAccountManagers { get; set; }

        public string OrganisationName { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string EmailAddress { get; set; }

        public string SelectedAccountType { get; set; }

        public bool IsDefaultAccountType
        {
            get => isDefaultAccountType;
            set
            {
                if (value)
                    SelectedAccountType = OrganisationFunction.Buyer.Name;
                isDefaultAccountType = value;
            }
        }

        public IEnumerable<SelectableRadioOption<string>> AccountTypeOptions => new List<SelectableRadioOption<string>>
        {
            new(OrganisationFunction.Buyer.DisplayName, OrganisationFunction.Buyer.Advice, OrganisationFunction.Buyer.Name),
            new(OrganisationFunction.AccountManager.DisplayName, OrganisationFunction.AccountManager.Advice, OrganisationFunction.AccountManager.Name),
        };

        public bool? IsActive { get; set; }

        public IEnumerable<SelectableRadioOption<bool>> StatusOptions => new List<SelectableRadioOption<bool>>
        {
            new("Active", true),
            new("Inactive", false),
        };

        public string ControllerName { get; set; }
    }
}
