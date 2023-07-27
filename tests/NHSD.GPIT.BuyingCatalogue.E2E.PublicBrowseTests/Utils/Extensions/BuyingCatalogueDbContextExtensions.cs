using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Extensions
{
    public static class BuyingCatalogueDbContextExtensions
    {
        public static ContractFlags GetContractFlags(this BuyingCatalogueDbContext context, int orderId)
        {
            if (context == null)
            {
                return null;
            }

            var output = context.ContractFlags.FirstOrDefault(x => x.OrderId == orderId);

            if (output != null)
            {
                return output;
            }

            output = new ContractFlags
            {
                OrderId = orderId,
            };

            context.ContractFlags.Add(output);
            context.SaveChanges();

            return output;
        }

        public static Contract GetContract(this BuyingCatalogueDbContext context, int orderId)
        {
            if (context == null)
            {
                return null;
            }

            var output = context.Contracts.FirstOrDefault(x => x.OrderId == orderId);

            if (output != null)
            {
                return output;
            }

            output = new Contract
            {
                OrderId = orderId,
            };

            context.Contracts.Add(output);
            context.SaveChanges();

            return output;
        }
    }
}
