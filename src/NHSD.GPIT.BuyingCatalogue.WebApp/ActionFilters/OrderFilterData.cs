using System.Text.RegularExpressions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;

/// <summary>
///     Partial to allow RegexGeneration.
/// </summary>
public static partial class OrderFilterData
{
    public static readonly Regex BasicCallOffIdRegex = MyRegex();

    [GeneratedRegex(@"/(C\d{1,6}-\d{2})/", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
