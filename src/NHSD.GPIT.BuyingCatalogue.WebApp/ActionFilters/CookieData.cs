using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class CookieData
    {
        public long? CreationDate { get; set; }

        public bool? Analytics { get; set; }
    }
}
