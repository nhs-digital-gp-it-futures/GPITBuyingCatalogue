using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public class AddNominatedOrganisationModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public IEnumerable<SelectListItem> PotentialOrganisations { get; set; }

        public string SelectedOrganisationId { get; set; }
    }
}
