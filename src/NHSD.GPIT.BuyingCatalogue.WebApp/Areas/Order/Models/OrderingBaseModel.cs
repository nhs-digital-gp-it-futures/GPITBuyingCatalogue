using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models
{
    [ExcludeFromCodeCoverage]
    public class OrderingBaseModel : NavBaseModel
    {
        public string Title { get; set; }

        public string OdsCode { get; set; }
    }
}
