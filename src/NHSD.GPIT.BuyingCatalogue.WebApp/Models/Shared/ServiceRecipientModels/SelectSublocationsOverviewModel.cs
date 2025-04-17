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
        string addOrChangeSublocationsHref,
        string backLinkHref)
        : this(isConfirm, sublocations, addOrChangeSublocationsHref, backLinkHref)
    {
        ProcessType = "competition";
        ParentName = competition.Organisation.Name;
        Caption = competition.Name;
    }

    public SelectSublocationsOverviewModel(
        bool isConfirm,
        Order order,
        IEnumerable<SublocationModel> sublocations,
        string addOrChangeSublocationsHref,
        string backLinkHref)
        : this(isConfirm, sublocations, addOrChangeSublocationsHref, backLinkHref)
    {
        ProcessType = "order";
        ParentName = order.OrderingParty.Name;
        Caption = order.Description;
    }

    private SelectSublocationsOverviewModel(
        bool isConfirm,
        IEnumerable<SublocationModel> sublocations,
        string addOrChangeSublocationsHref,
        string backLinkHref)
    {
        Sublocations = sublocations.ToList();
        AddOrChangeSublocationsHref = addOrChangeSublocationsHref;

        BackLink = backLinkHref;
        SetConditionalTitleAndAdvice(isConfirm);
    }

    public string ProcessType { get; init; }

    public string AddOrChangeSublocationsHref { get; init; }

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
