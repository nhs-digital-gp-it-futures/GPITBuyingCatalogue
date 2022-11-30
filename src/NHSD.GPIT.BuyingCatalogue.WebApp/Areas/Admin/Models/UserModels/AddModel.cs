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

        public string Email { get; set; }

        public string SelectedAccountType { get; set; }

        public string SelectedOrganisationId { get; set; }

        public IEnumerable<SelectableRadioOption<string>> AccountTypeOptions => new List<SelectableRadioOption<string>>
        {
            new(OrganisationFunction.Buyer.DisplayName, OrganisationFunction.Buyer.InternalAdvice, $"{OrganisationFunction.Buyer.Name}"),
            new(OrganisationFunction.AccountManager.DisplayName, OrganisationFunction.AccountManager.InternalAdvice, $"{OrganisationFunction.AccountManager.Name}"),
            new(OrganisationFunction.Authority.DisplayName, OrganisationFunction.Authority.InternalAdvice, $"{OrganisationFunction.Authority.Name}"),
        };

        public bool? IsActive { get; set; }

        public IEnumerable<SelectableRadioOption<bool>> StatusOptions => new List<SelectableRadioOption<bool>>
        {
            new("Active", true),
            new("Inactive", false),
        };

        public IEnumerable<SelectListItem> Organisations { get; set; }
    }
}
