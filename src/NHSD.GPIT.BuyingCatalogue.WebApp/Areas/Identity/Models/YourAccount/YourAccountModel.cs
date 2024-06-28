using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount
{
    public class YourAccountModel : YourAccountBaseModel
    {
        public YourAccountModel(Organisation organisation)
        {
            Organisation = organisation ?? throw new ArgumentNullException(nameof(organisation));
        }

        public Organisation Organisation { get; set; }

        public Address OrganisationAddress => Organisation.Address;

        public override int Index => 0;
    }
}
