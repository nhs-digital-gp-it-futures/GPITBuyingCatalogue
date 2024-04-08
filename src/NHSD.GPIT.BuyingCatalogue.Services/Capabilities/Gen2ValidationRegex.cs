using System.Text.RegularExpressions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities;

/// <summary>
/// Contains source-generated Regex validators.
/// </summary>
internal static partial class Gen2ValidationRegex
{
    private const int MillisecondsTimeout = 500;

    [GeneratedRegex(
        "^[0-9]*-[0-9]{3}$",
        RegexOptions.Singleline,
        MillisecondsTimeout)]
    internal static partial Regex CatalogueItemIdRegex();

    [GeneratedRegex(
        "^A[0-9]{2,3}$",
        RegexOptions.Singleline,
        MillisecondsTimeout)]
    internal static partial Regex AdditionalServiceRegex();

    [GeneratedRegex(
        "^C[0-9]*$",
        RegexOptions.Singleline,
        MillisecondsTimeout)]
    internal static partial Regex CapabilityIdRegex();
}
