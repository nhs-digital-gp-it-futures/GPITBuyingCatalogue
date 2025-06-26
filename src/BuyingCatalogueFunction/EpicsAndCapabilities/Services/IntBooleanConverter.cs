using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Services;

public class IntBooleanConverter : BooleanConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        return text.Trim() switch
        {
            "0" => false,
            "1" => true,
            _ => base.ConvertFromString(text, row, memberMapData),
        };
    }
}
