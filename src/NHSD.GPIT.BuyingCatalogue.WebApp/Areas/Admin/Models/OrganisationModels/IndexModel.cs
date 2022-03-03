using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public sealed class IndexModel : NavBaseModel
    {
        public IndexModel(string searchTerm, IList<OrganisationModel> organisations)
        {
            SearchTerm = searchTerm;
            Organisations = organisations;
        }

        public string SearchTerm { get; set; }

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
