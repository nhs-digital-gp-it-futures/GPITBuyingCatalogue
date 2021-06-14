using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class InteroperabilityModel : SolutionDisplayBaseModel
    {
        public IList<IntegrationModel> Integrations { get; set; } = new List<IntegrationModel>();

        public override int Index => 6;

        public string TextDescriptionsProvided()
        {
            if (Integrations == null || !Integrations.Any())
                return "No integration yet";

            var start = Integrations.Count == 1
                ? "There is one type of integration"
                : $"There are {Integrations.Count.ToEnglish()} types";
            return $"{start} of integrations specified and assured by the NHS.";
        }
    }
}
