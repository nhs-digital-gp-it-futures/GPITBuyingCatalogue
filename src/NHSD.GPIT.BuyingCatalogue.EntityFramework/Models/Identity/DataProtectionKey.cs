#nullable disable

using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity
{
    [ExcludeFromCodeCoverage]
    public partial class DataProtectionKey
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public string Xml { get; set; }
    }
}
