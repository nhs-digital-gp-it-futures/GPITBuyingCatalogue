using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Domain name settings.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class DomainNameSettings
    {
        public string DomainName { get; set; }
    }
}
