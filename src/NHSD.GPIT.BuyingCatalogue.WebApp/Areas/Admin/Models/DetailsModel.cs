using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class DetailsModel : NavBaseModel
    {
        public DetailsModel()
        {
        }

        public DetailsModel(Organisation organisation, List<AspNetUser> users)
        {
            Organisation = organisation;
            Users = users;
            BackLink = "/admin/organisations";
        }

        public Organisation Organisation { get; set; }

        public List<AspNetUser> Users { get; set; }

        public Address OrganisationAddress { get { return Organisation.GetAddress(); } }
    }
}
