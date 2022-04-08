using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class AccountTypeModel : NavBaseModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string SelectedAccountType { get; set; }

        public IList<SelectListItem> AccountTypeOptions => new List<SelectListItem>
        {
            new(OrganisationFunction.BuyerName, $"{OrganisationFunction.BuyerName}"),
            new("Admin", $"{OrganisationFunction.AuthorityName}"),
        };
    }
}
