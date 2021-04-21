using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class SolutionExtensions
    {
        public static ClientApplication GetClientApplication(this Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solution.ClientApplication))
                return new ClientApplication();

            return JsonConvert.DeserializeObject<ClientApplication>(solution.ClientApplication);
        }

        public static string[] GetFeatures(this Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solution.ClientApplication))
                return new string[0];

            return JsonConvert.DeserializeObject<string[]>(solution.Features);
        }

        public static Hosting GetHosting(this Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solution.ClientApplication))
                return new Hosting();

            return JsonConvert.DeserializeObject<Hosting>(solution.Hosting);
        }
    }
}
