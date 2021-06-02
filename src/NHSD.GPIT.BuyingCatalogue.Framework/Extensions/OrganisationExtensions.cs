using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class OrganisationExtensions
    {
        public static Address GetAddress(this Organisation organisation)
        {
            if (string.IsNullOrWhiteSpace(organisation.Address))
                return new Address();

            return JsonConvert.DeserializeObject<Address>(organisation.Address);
        }
    }
}
