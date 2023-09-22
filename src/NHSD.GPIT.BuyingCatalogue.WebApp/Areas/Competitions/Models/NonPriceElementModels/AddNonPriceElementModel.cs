using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class AddNonPriceElementModel : NavBaseModel
{
    public AddNonPriceElementModel()
    {
    }

    public AddNonPriceElementModel(
        Competition competition)
    {
        CompetitionName = competition.Name;
        AvailableNonPriceElements = competition.NonPriceElements.GetAvailableNonPriceElements()
            .Select(x => new SelectOption<NonPriceElement>(x.EnumMemberName(), x))
            .OrderBy(x => x.Text)
            .ToList();
    }

    public string CompetitionName { get; set; }

    public List<SelectOption<NonPriceElement>> AvailableNonPriceElements { get; set; }
}
