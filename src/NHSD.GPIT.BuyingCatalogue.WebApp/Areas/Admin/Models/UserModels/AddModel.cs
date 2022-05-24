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

        public IList<SelectListItem> AccountTypeOptions => new List<SelectListItem>
        {
            new(OrganisationFunction.BuyerName, $"{OrganisationFunction.BuyerName}"),
            new("Admin", $"{OrganisationFunction.AuthorityName}"),
        };

        public IEnumerable<SelectListItem> Organisations { get; set; }
    }
}
