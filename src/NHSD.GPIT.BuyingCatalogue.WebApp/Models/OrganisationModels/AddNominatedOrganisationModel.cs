using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public class AddNominatedOrganisationModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public IEnumerable<SelectListItem> PotentialOrganisations { get; set; }

        public string SelectedOrganisationId { get; set; }
    }
}
