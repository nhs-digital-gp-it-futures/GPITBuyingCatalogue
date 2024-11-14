using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public static class OrderStatusExtensions
    {
        public static string AsFormattedString(this OrderStatus orderStatus)
        {
            return orderStatus.AsString(EnumFormat.EnumMemberValue)?.Replace(" ", "-", StringComparison.InvariantCulture).ToLowerInvariant() ?? "unknown-status";
        }
    }
}
