using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface ICreateOrderItemValidator
    {
        AggregateValidationResult Validate(Order order, CreateOrderItemModel model, CatalogueItemType itemType);
    }
}
