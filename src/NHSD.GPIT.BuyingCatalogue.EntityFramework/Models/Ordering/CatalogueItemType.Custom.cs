using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class CatalogueItemType
    {
        public string DisplayName() => Name;

        public TimeUnit InferEstimationPeriod(ProvisioningType provisioningType, TimeUnit estimationPeriod)
        {
            return Name switch
            {
                "AssociatedService" => provisioningType.Name == "On Demand"
                    ? provisioningType.InferEstimationPeriod(estimationPeriod)
                    : null,
                _ => provisioningType.InferEstimationPeriod(estimationPeriod),
            };
        }

        public void MarkOrderSectionAsViewed(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            switch (Name)
            {
                case "Solution":
                    order.OrderProgress.CatalogueSolutionsViewed = true;
                    break;

                case "AdditionalService":
                    order.OrderProgress.AdditionalServicesViewed = true;
                    break;

                case "AssociatedService":
                    order.OrderProgress.AssociatedServicesViewed = true;
                    break;

                default:
                    throw new ArgumentException($"{nameof(Name)} must be a valid {nameof(CatalogueItemType)}", nameof(Name));
            }
        }
    }
}
