using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public sealed class ListOrganisationsModel : NavBaseModel
    {
        public ListOrganisationsModel(IList<OrganisationModel> organisations)
        {
            Organisations = organisations;
        }

        public IList<OrganisationModel> Organisations { get; }

        public Dictionary<string, string> BreadCrumb()
        {
            return new()
            {
                { "Home", "/admin" },
            };
        }
    }
}
