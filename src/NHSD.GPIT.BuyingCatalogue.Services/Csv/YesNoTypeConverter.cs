using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv;

public sealed class YesNoTypeConverter : TypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        => string.Equals(text, "Yes", StringComparison.OrdinalIgnoreCase);

    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        return value is not bool boolValue
            ? base.ConvertToString(value, row, memberMapData)
            : boolValue.ToYesNo();
    }
}
