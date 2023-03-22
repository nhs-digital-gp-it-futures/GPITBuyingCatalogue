using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    [ExcludeFromCodeCoverage]
    public class ServiceRecipient
    {
        public string Name { get; set; }

        public string OrgId { get; set; }

        public string Status { get; set; }

        public string OrganisationRoleId { get; set; }
    }
}
