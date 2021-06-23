using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsBreadcumbs;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class ListOrganisationsModel : NavBaseModel
    {
        public ListOrganisationsModel(IList<OrganisationModel> organisations)
        {
            Organisations = organisations;
        }

        public IList<OrganisationModel> Organisations { get; private set; }

        public List<NhsBreadcrumbModel> Breadcrumb()
        {
            return new List<NhsBreadcrumbModel>()
            {
                new NhsBreadcrumbModel {
                    Action = nameof(HomeController.Index),
                    Controller = "admin",
                    Text = "Home",
                },
            };
        }
    }
}
