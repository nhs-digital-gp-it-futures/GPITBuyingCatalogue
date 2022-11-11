using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public sealed class AddUserModel : NavBaseModel
    {
        public AddUserModel()
        {
        }

        public AddUserModel(Organisation organisation, AspNetUser user)
            : this(organisation)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAddress = user.Email;
            SelectedAccountType = user.AspNetUserRoles.First()?.Role?.Name;
            IsActive = !user.Disabled;
        }

        public AddUserModel(Organisation organisation)
        {
            OrganisationName = organisation.Name;
        }

        public string Title
        {
            get
            {
                return UserId == 0 ? "Add user" : "Edit user";
            }
        }

        public int UserId { get; set; }

        public string OrganisationName { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string EmailAddress { get; set; }

        public string SelectedAccountType { get; set; }

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
