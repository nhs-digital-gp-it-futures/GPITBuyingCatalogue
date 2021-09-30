using System.Collections.Generic;
using System.Text.Json;
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

            return JsonSerializer.Deserialize<ClientApplication>(solution.ClientApplication, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static string[] GetFeatures(this Solution solution) => string.IsNullOrWhiteSpace(solution.Features)
            ? System.Array.Empty<string>()
            : JsonSerializer.Deserialize<string[]>(solution.Features, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        public static List<Integration> GetIntegrations(this Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solution.Integrations))
                return new List<Integration>();

            return JsonSerializer.Deserialize<List<Integration>>(solution.Integrations, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
