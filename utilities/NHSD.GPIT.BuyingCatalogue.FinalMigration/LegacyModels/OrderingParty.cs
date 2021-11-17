using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class OrderingParty
    {
        public Guid Id { get; set; }
        public string OdsCode { get; set; }
        public string Name { get; set; }
        public int? AddressId { get; set; }
    }
}
