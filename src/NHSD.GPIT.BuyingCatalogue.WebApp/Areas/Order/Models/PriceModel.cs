using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models
{
    [ExcludeFromCodeCoverage]
    public class PriceModel
    {
        public int CataloguePriceId { get; set; }

        public string Description { get; set; }
    }
}
