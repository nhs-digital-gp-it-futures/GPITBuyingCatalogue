using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public sealed class OdsSettings
    {
        public IReadOnlyList<OrganisationRoleSettings> BuyerOrganisationRoles { get; init; }

        public string SubLocationRoleId { get; set; }

        public string IsCommissionedByRelType { get; set; }

        public string InGeographyOfRelType { get; set; }

        public OrganisationType GetOrganisationType(string primaryRoleId)
        {
            if (string.IsNullOrWhiteSpace(primaryRoleId))
                throw new ArgumentNullException(nameof(primaryRoleId));

            var role = BuyerOrganisationRoles.FirstOrDefault(x => x.PrimaryRoleId == primaryRoleId);

            if (role is null)
                throw new ArgumentException("Not a buyer role", nameof(primaryRoleId));

            return role.OrganisationType;
        }

        public string GetPrimaryRoleId(OrganisationType orgType)
        {
            var role = BuyerOrganisationRoles.FirstOrDefault(x => x.OrganisationType == orgType);

            if (role is null)
                throw new ArgumentException("Not a buyer role", nameof(orgType));

            return role.PrimaryRoleId;
        }
    }
}
