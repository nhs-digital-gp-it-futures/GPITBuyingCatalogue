using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class MergerOrderCsvModel : FullOrderCsvModel
    {
        public string ServiceRecipientToClose { get; set; }

        public string ServiceRecipientToRetain { get; set; }
    }
}
