using System.Diagnostics;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

public static class HeadingSizeExtensions
{
    public static string ToHeading(this HeadingSize headingSize) => headingSize switch
    {
        HeadingSize.ExtraSmall => "nhsuk-heading-xs",
        HeadingSize.Small => "nhsuk-heading-s",
        HeadingSize.Medium => "nhsuk-heading-m",
        HeadingSize.Large => "nhsuk-heading-l",
        HeadingSize.ExtraLarge => "nhsuk-heading-xl",
        _ => throw new UnreachableException(),
    };
}
