using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class AddModel : NavBaseModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TelephoneNumber { get; set; }

        public string Email { get; set; }

        public string SelectedAccountStatusId { get; set; }

        public AccountStatus SelectedAccountStatus => SelectedAccountStatusId == $"{AccountStatus.Active}"
            ? AccountStatus.Active
            : AccountStatus.Inactive;

        public string SelectedAccountType { get; set; }

        public string SelectedOrganisationId { get; set; }

        public IList<SelectListItem> AccountStatusOptions => new List<SelectListItem>
        {
            new(Enum.GetName(AccountStatus.Active), $"{AccountStatus.Active}"),
            new(Enum.GetName(AccountStatus.Inactive), $"{AccountStatus.Inactive}"),
        };

        public IList<SelectListItem> AccountTypeOptions => new List<SelectListItem>
        {
            new(OrganisationFunction.BuyerName, $"{OrganisationFunction.BuyerName}"),
            new("Admin", $"{OrganisationFunction.AuthorityName}"),
        };

        public IEnumerable<SelectListItem> Organisations { get; set; }
    }
}
