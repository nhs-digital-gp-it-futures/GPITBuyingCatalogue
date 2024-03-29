﻿using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class OdsResponseOrganisation
    {
        public string Name { get; init; }

        public string Status { get; init; }

        public OrgId OrgId { get; init; }

        public GeoLoc GeoLoc { get; init; }

        public OdsResponseRoles Roles { get; init; }
    }
}
