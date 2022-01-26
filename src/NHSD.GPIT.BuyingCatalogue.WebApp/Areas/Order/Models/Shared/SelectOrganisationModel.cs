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

        public SelectOrganisationModel(string currentOdsCode, List<Organisation> organisations)
        {
            Title = "Which organisation are you looking for?";
            OdsCode = currentOdsCode;
            AvailableOrganisations = organisations;
            SelectedOrganisation = organisations.Single(o => o.OdsCode.EqualsIgnoreCase(currentOdsCode)).OdsCode;
        }

        public List<Organisation> AvailableOrganisations { get; set; }

        public string SelectedOrganisation { get; set; }
    }
}
