using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage
{
    public class TriageDueDiligenceModel : NavBaseModel
    {
        public TriageDueDiligenceModel()
        {
        }

        public TriageDueDiligenceModel(Organisation organisation)
        {
            OrganisationName = organisation.Name;
        }

        public string OrganisationName { get; set; }

        public string Title { get; set; }

        public string Advice { get; set; }

        public bool? Selected { get; set; }

        public OrderTriageValue? Option { get; set; }

        public IEnumerable<SelectOption<string>> SelectListItems => new[]
        {
            new SelectOption<string>(true.ToYesNo(), true.ToString()),
            new SelectOption<string>(false.ToYesNo(), false.ToString()),
        };
    }
}
