using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class SolutionExtensions
    {
        public static IEnumerable<string> GetFeatures(this Solution solution)
        {
            ArgumentNullException.ThrowIfNull(solution);

            return string.IsNullOrWhiteSpace(solution.Features)
                ? Array.Empty<string>()
                : JsonDeserializer.Deserialize<string[]>(solution.Features);
        }
    }
}
