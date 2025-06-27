using System.Text.RegularExpressions;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components;

internal static partial class RegularExpressions
{
    private const int MillisecondsTimeout = 500;

    [GeneratedRegex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+", RegexOptions.None, MillisecondsTimeout)]
    internal static partial Regex KebabNameRegex();
}
