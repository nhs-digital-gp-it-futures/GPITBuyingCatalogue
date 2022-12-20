using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class PriceModel
    {
        public int CataloguePriceId { get; set; }

        public string Description { get; set; }
    }
}
