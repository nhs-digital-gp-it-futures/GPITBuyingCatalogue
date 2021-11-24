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
            Organisation = organisation;
            AvailableOrganisations = availableOrganisations;
        }

        public Organisation Organisation { get; set; }

        public List<Organisation> AvailableOrganisations { get; set; }

        public int SelectedOrganisation { get; set; }
    }
}
