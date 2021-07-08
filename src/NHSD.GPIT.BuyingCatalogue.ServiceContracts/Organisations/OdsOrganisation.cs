using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    [ExcludeFromCodeCoverage]
    public sealed class OdsOrganisation
    {
        public string OdsCode { get; set; }

        public string OrganisationName { get; set; }

        public string PrimaryRoleId { get; set; }

        public Address Address { get; set; }

        public bool IsActive { get; set; }

        public bool IsBuyerOrganisation { get; set; }
    }
}
