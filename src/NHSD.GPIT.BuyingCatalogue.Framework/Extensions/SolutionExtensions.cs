using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class SolutionExtensions
    {
        public static ClientApplication GetClientApplication(this Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solution.ClientApplication))
                return new ClientApplication();

            return JsonDeserializer.Deserialize<ClientApplication>(solution.ClientApplication);
        }

        public static string[] GetFeatures(this Solution solution) => string.IsNullOrWhiteSpace(solution.Features)
            ? System.Array.Empty<string>()
            : JsonDeserializer.Deserialize<string[]>(solution.Features);

        public static List<Integration> GetIntegrations(this Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solution.Integrations))
                return new List<Integration>();

            return JsonDeserializer.Deserialize<List<Integration>>(solution.Integrations);
        }
    }
}
