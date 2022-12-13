using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class UserDetailsModel : NavBaseModel
    {
        public UserDetailsModel()
        {
        }

        public UserDetailsModel(AspNetUser user)
        {
            SelectedOrganisationId = $"{user.PrimaryOrganisationId}";
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            SelectedAccountType = user.GetRoleName();
            IsActive = !user.Disabled;
        }

        public string Title
        {
            get
            {
                return UserId == 0 ? "Add user" : "Edit user";
            }
        }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string SelectedAccountType { get; set; }

        public string SelectedOrganisationId { get; set; }

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
