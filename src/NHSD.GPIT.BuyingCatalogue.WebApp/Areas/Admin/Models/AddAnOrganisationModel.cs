using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class AddAnOrganisationModel : NavBaseModel
    {
        public AddAnOrganisationModel()
        {
        }

        public AddAnOrganisationModel(Organisation organisation, List<Organisation> availableOrganisations)
        {
            OrganisationId = organisation.Id;
            OrganisationName = organisation.Name;
            AvailableOrganisations = availableOrganisations;
        }

        public int OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public List<Organisation> AvailableOrganisations { get; set; }

        public int? SelectedOrganisation { get; set; }
    }
}
