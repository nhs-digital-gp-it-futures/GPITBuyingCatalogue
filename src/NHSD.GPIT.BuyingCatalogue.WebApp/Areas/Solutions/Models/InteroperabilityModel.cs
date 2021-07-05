using System.Collections.Generic;
using System.Linq;

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

            return "IM1 and GP Connect offer integrations specified and assured by the NHS.";
        }
    }
}
