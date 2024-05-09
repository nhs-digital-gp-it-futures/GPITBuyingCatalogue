using System;
using System.Diagnostics;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

public static class ColumnWidthExtensions
{
    public static string ToClass(this ColumnWidth columnWidth) => columnWidth switch
    {
        ColumnWidth.Half => "nhsuk-grid-column-one-half",
        ColumnWidth.OneThird => "nhsuk-grid-column-one-third",
        _ => throw new ArgumentOutOfRangeException(nameof(columnWidth)),
    };
}
