using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class SplitOrderCsvModel : FullOrderCsvModel
    {
        public string ServiceRecipientToSplit { get; set; }

        public string ServiceRecipientToRetain { get; set; }
    }
}
