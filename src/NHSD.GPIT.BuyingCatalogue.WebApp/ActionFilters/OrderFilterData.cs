using System.Text.RegularExpressions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;

/// <summary>
///     Partial to allow RegexGeneration.
///     Will check for call off id, but has to be within slashes (/).
///     Will need updating if call off id is used on action where id is the last element (none at present).
/// </summary>
public static partial class OrderFilterData
{
    public static readonly Regex BasicCallOffIdRegex = MyRegex();

    [GeneratedRegex(@"\/(C\d{1,6}-\d{2})\/", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
