using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class AddAnOrganisationModel : NavBaseModel
    {
        public AddAnOrganisationModel()
        {
        }

        public AddAnOrganisationModel(Organisation organisation, List<Organisation> availableOrganisations)
        {
            Organisation = organisation;
            AvailableOrganisations = availableOrganisations;
            BackLink = $"/admin/organisations/{organisation.OrganisationId}";
        }

        public Organisation Organisation { get; set; }

        public List<Organisation> AvailableOrganisations { get; set; }

        public Guid SelectedOrganisation { get; set; }
    }
}
