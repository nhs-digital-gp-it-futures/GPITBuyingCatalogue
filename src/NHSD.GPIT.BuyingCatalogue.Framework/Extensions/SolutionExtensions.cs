using System.Collections.Generic;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

        public static List<Integration> GetIntegrations(this Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solution.Integrations))
                return new List<Integration>();

            return JsonConvert.DeserializeObject<List<Integration>>(solution.Integrations);
        }
    }
}
