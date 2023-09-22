using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class NonPriceElementsModel : NavBaseModel
{
    public NonPriceElementsModel(
        Competition competition)
    {
        InternalOrgId = competition.Organisation.InternalIdentifier;
        CompetitionId = competition.Id;

        CompetitionName = competition.Name;
        NonPriceElements = competition.NonPriceElements;
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CompetitionName { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public bool HasAllNonPriceElements() =>
        NonPriceElements.HasAllNonPriceElements();

    public bool HasAnyNonPriceElements() =>
        NonPriceElements.HasAnyNonPriceElements();
}
