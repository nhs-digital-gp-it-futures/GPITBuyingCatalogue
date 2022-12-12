using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public class AddNominatedOrganisationModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public IEnumerable<SelectOption<string>> PotentialOrganisations { get; set; }

        public string SelectedOrganisationId { get; set; }
    }
}
