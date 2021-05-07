using System.Diagnostics.CodeAnalysis;

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
