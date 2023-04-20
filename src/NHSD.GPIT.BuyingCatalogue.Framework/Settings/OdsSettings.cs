using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class OdsSettings
    {
        public Uri ApiBaseUrl { get; init; }

        public IReadOnlyList<string> BuyerOrganisationRoleIds { get; init; }

        public string GpPracticeRoleId { get; set; }

        public string IcbRoleId { get; set; }

        public string SubLocationRoleId { get; set; }

        public string IsCommissionedByRelType { get; set; }

        public string InGeographyOfRelType { get; set; }

        public int GetChildOrganisationSearchLimit { get; set; }
    }
}
