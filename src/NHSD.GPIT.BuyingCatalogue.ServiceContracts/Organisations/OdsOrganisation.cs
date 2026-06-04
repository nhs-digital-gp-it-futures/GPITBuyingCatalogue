using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;

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

        public static OdsOrganisation From(EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation) =>
            new()
            {
                IsActive = organisation.IsActive,
                OdsCode = organisation.Id,
                OrganisationName = organisation.Name,
                PrimaryRoleId = organisation.Roles.FirstOrDefault(x => x.IsPrimaryRole)?.RoleId,
                Address = new()
                {
                    Line1 = organisation.AddressLine1,
                    Line2 = organisation.AddressLine2,
                    Line3 = organisation.AddressLine3,
                    Town = organisation.Town,
                    County = organisation.County,
                    Postcode = organisation.Postcode,
                    Country = organisation.Country,
                },
            };
    }
}
