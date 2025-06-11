using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public sealed class SelectSublocationsOverviewModel : NavBaseModel
{
    public SelectSublocationsOverviewModel()
    {
    }

    public SelectSublocationsOverviewModel(
        bool isConfirm,
        Competition competition,
        IEnumerable<SublocationModel> sublocations,
        string addOrChangeSublocationsLink,
        string backLink)
        : this(sublocations, addOrChangeSublocationsLink, backLink)
    {
        ProcessType = "competition";
        ParentName = competition.Organisation.Name;
        Caption = competition.Name;
        SetConditionalTitleAndAdvice(isConfirm);
    }

    public SelectSublocationsOverviewModel(
        bool isConfirm,
        Order order,
        IEnumerable<SublocationModel> sublocations,
        string addOrChangeSublocationsLink,
        string backLink)
        : this(sublocations, addOrChangeSublocationsLink, backLink)
    {
        ProcessType = "order";
        ParentName = order.OrderingParty.Name;
        Caption = order.Description;
        SetConditionalTitleAndAdvice(isConfirm);
    }

    private SelectSublocationsOverviewModel(
        IEnumerable<SublocationModel> sublocations,
        string addOrChangeSublocationsLink,
        string backLink)
    {
        Sublocations = sublocations.ToList();
        AddOrChangeSublocationsLink = addOrChangeSublocationsLink;

        BackLink = backLink;
    }

    public string ProcessType { get; init; }

    public string AddOrChangeSublocationsLink { get; init; }

    public string ParentName { get; init; }

    public List<SublocationModel> Sublocations { get; init; } = [];

    private void SetConditionalTitleAndAdvice(bool isConfirm)
    {
        Title = isConfirm ? "Confirm sublocations" : "Add sublocations";
        Advice = isConfirm
            ? $"Select a sublocation to amend the organisations in this {ProcessType}"
            : $"Select a sublocation to add organisations to this {ProcessType}";
    }
}
