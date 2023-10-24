using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class CompetitionReviewCriteriaModel : NavBaseModel
{
    public CompetitionReviewCriteriaModel()
    {
    }

    public CompetitionReviewCriteriaModel(Competition competition)
    {
        InternalOrgId = competition.Organisation.InternalIdentifier;
        CompetitionId = competition.Id;

        CompetitionName = competition.Name;
        CompetitionWeights = competition.Weightings;
        NonPriceElements = competition.NonPriceElements;
        HasReviewedCriteria = competition.HasReviewedCriteria;

        NonPriceWeights = competition.NonPriceElements.GetNonPriceElements()
            .OrderBy(x => x.ToString())
            .ToDictionary(x => x, x => competition.NonPriceElements.GetNonPriceWeight(x));
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CompetitionName { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public Weightings CompetitionWeights { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public Dictionary<NonPriceElement, int?> NonPriceWeights { get; set; }
}
