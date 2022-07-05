using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Extensions
{
    public static class TempDataExtensions
    {
        public const string LineIdKey = "temp-data-line-id";

        public static int NextLineId(this ITempDataDictionary tempData)
        {
            var output = (int)(tempData[LineIdKey] ?? 0);

            tempData[LineIdKey] = ++output;

            return output;
        }
    }
}
