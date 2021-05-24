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
    }
}
