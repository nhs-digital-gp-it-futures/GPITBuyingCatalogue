using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsBreadcumbs;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class DetailsModel : NavBaseModel
    {
        public DetailsModel()
        {
        }

        public DetailsModel(Organisation organisation, List<AspNetUser> users, List<Organisation> relatedOrganisations)
        {
            Organisation = organisation;
            Users = users;
            RelatedOrganisations = relatedOrganisations;
            BackLink = "/admin/organisations";
        }

        public Organisation Organisation { get; set; }

        public List<AspNetUser> Users { get; set; }

        public List<Organisation> RelatedOrganisations { get; set; }

        public Address OrganisationAddress
        {
            get { return Organisation.GetAddress(); }
        }

        public List<NhsBreadcrumbModel> Breadcrumb()
        {
            return new List<NhsBreadcrumbModel>()
            {
                new NhsBreadcrumbModel {
                    Controller = "admin",
                    Action = nameof(HomeController.Index),
                    Text = "Home",
                },
                new NhsBreadcrumbModel {
                    Controller = "admin",
                    Action = "buyer-organisations",
                    Text = "Manage Catalogue Solutions",
                },
            };
        }
    }
}
