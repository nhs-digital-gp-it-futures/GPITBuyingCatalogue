using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public class OrderingBaseModel : NavBaseModel
    {
        public string InternalOrgId { get; set; }
    }
}
