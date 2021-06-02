using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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

        public static string[] GetFeatures(this Solution solution) => string.IsNullOrWhiteSpace(solution.Features)
            ? System.Array.Empty<string>()
            : JsonConvert.DeserializeObject<string[]>(solution.Features);

        public static Hosting GetHosting(this Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solution.Hosting))
                return new Hosting();

            return JsonConvert.DeserializeObject<Hosting>(solution.Hosting);
        }
    }
}
