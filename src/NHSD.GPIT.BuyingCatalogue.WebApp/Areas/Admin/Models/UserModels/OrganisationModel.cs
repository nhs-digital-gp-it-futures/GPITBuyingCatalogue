using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class OrganisationModel : NavBaseModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string SelectedOrganisationId { get; set; }

        public IEnumerable<SelectListItem> Organisations { get; set; }
    }
}
