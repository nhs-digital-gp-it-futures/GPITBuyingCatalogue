using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class OrganisationExtensions
    {
        public static Address GetAddress(this Organisation organisation)
        {
            if (string.IsNullOrWhiteSpace(organisation?.Address)) // MJRTODO
                return new Address();

            return JsonConvert.DeserializeObject<Address>(organisation.Address);
        }
    }
}
