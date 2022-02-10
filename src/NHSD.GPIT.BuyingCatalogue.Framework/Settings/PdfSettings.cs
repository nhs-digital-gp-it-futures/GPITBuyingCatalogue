using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Pdf settings.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class PdfSettings
    {
        public bool UseSslForPdf { get; set; }
    }
}
