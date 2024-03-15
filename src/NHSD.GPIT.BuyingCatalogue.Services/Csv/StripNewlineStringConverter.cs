using System;
using System.Diagnostics.CodeAnalysis;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv;

[ExcludeFromCodeCoverage]
internal sealed class StripNewlineStringConverter : StringConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return base.ConvertFromString(RemoveNewLines(text), row, memberMapData);
    }

    private static string RemoveNewLines(string text) =>
        text.Replace("\r", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("\n", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("\r\n", string.Empty, StringComparison.OrdinalIgnoreCase);
}
