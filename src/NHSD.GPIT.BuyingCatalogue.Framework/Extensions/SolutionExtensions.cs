using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class SolutionExtensions
    {
        public static IEnumerable<string> GetFeatures(this Solution solution)
        {
            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            return string.IsNullOrWhiteSpace(solution.Features)
                ? Array.Empty<string>()
                : JsonDeserializer.Deserialize<string[]>(solution.Features);
        }

        public static ICollection<Integration> GetIntegrations(this Solution solution)
        {
            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            return string.IsNullOrWhiteSpace(solution.Integrations)
                ? Enumerable.Empty<Integration>().ToList()
                : JsonDeserializer.Deserialize<List<Integration>>(solution.Integrations);
        }
    }
}
