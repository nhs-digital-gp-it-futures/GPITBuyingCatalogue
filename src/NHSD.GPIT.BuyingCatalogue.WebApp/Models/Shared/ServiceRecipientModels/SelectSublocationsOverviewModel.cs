using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public sealed class SelectSublocationsOverviewModel : NavBaseModel
{
    public SelectSublocationsOverviewModel()
    {
    }

    public SelectSublocationsOverviewModel(
        bool isConfirm,
        Competition competition,
        IReadOnlyList<SublocationModel> sublocations,
        string addOrChangeSublocationsHref,
        string backLinkHref)
    {
        ProcessType = "competition";
        Sublocations = sublocations;
        ParentName = competition.Organisation.Name;
        AddOrChangeSublocationsHref = addOrChangeSublocationsHref;

        BackLink = backLinkHref;
        Caption = competition.Name;
        SetConditionalTitleAndAdvice(isConfirm);
    }

    public string ProcessType { get; init; }

    public string AddOrChangeSublocationsHref { get; init; }

    public string ParentName { get; init; }

    public IReadOnlyList<SublocationModel> Sublocations { get; init; } = [];

    private void SetConditionalTitleAndAdvice(bool isConfirm)
    {
        Title = isConfirm ? "Confirm sublocations" : "Add sublocations";
        Advice = isConfirm
            ? $"Select a sublocation to amend the organisations in this {ProcessType}"
            : $"Select a sublocation to add organisations to this {ProcessType}";
    }
}
