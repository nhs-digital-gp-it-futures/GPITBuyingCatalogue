using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared
{
    public class SelectOrganisationModel : OrderingBaseModel
    {
        public SelectOrganisationModel()
        {
        }

        public SelectOrganisationModel(string currentInternalOrgId, List<Organisation> organisations)
        {
            Title = "Which organisation are you looking for?";
            InternalOrgId = currentInternalOrgId;
            AvailableOrganisations = organisations;
            SelectedOrganisation = organisations.Single(o => o.InternalIdentifier.EqualsIgnoreCase(currentInternalOrgId)).InternalIdentifier;
        }

        public List<Organisation> AvailableOrganisations { get; set; }

        public string SelectedOrganisation { get; set; }
    }
}
