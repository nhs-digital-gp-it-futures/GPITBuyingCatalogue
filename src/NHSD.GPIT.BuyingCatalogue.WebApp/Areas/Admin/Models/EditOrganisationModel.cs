using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class EditOrganisationModel : NavBaseModel
    {
        public EditOrganisationModel()
        {
        }

        public EditOrganisationModel(Organisation organisation)
        {
            Organisation = organisation ?? throw new ArgumentNullException(nameof(organisation));
            CatalogueAgreementSigned = organisation.CatalogueAgreementSigned;
            OrganisationAddress = organisation.Address;
        }

        public Organisation Organisation { get; set; }

        public Address OrganisationAddress { get; set; }

        public bool CatalogueAgreementSigned { get; set; }
    }
}
