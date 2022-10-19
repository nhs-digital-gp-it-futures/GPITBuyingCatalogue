using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Models
{
    public sealed class ManageAccountModel : NavBaseModel
    {
        public ManageAccountModel()
        {
        }

        public ManageAccountModel(Organisation organisation, List<AspNetUser> users, List<Organisation> relatedOrganisations)
        {
            Organisation = organisation ?? throw new ArgumentNullException(nameof(organisation));
            CatalogueAgreementText = organisation.CatalogueAgreementSigned ? "Organisation End User Agreement has been signed" : "Organisation End User Agreement has not been signed";
            Users = users;
            RelatedOrganisations = relatedOrganisations;
        }

        public Organisation Organisation { get; set; }

        public List<AspNetUser> Users { get; set; }

        public List<Organisation> RelatedOrganisations { get; set; }

        public Address OrganisationAddress => Organisation.Address;

        public string CatalogueAgreementText { get; set; }
    }
}
