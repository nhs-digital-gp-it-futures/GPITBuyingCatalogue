using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

public class FilteredDirectAwardModel : NavBaseModel
{
    public FilteredDirectAwardModel(
        Competition competition)
    {
        CompetitionName = competition.Name;
        InternalOrgId = competition.Organisation.InternalIdentifier;
        CompetitionId = competition.Id;
    }

    public FilteredDirectAwardModel(
        Competition competition,
        FilterDetailsModel filterDetails,
        ICollection<CompetitionSolution> nonShortlistedSolutions)
        : this(competition)
    {
        NonShortlistedSolutions = nonShortlistedSolutions;

        FilterDetailsModel = new ReviewFilterModel(filterDetails);
    }

    public string CompetitionName { get; set; }

    public string PdfUrl { get; set; }

    public ReviewFilterModel FilterDetailsModel { get; set; }

    public ICollection<CompetitionSolution> NonShortlistedSolutions { get; set; }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }
}
