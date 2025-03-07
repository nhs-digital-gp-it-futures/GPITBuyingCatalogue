using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public sealed class SelectSublocationsOverviewModel : NavBaseModel
{
    public SelectSublocationsOverviewModel(
        bool isConfirm,
        Competition competition,
        List<SublocationModel> sublocations,
        string addOrChangeSublocationsHref)
    {
        ProcessType = "competition";
        EntityNameForCaption = competition.Name;
        Sublocations = sublocations;
        ParentName = competition.Organisation.Name;

        SetTitleParams(isConfirm);

        AddOrChangeSublocationsHref = addOrChangeSublocationsHref;
    }

    public SelectSublocationsOverviewModel(bool isConfirm, Order order, Organisation organisation)
    {
        ProcessType = "order";
        EntityNameForCaption = order.Description;

        SetTitleParams(isConfirm);
    }

    public string ProcessType { get; set; }

    public string EntityNameForCaption { get; set; }

    public string AddOrChangeSublocationsHref { get; set; }

    public string ParentName { get; set; }

    public ICollection<SublocationModel> Sublocations { get; set; }

    private void SetTitleParams(bool isConfirm)
    {
        Title = isConfirm ? "Confirm sublocations" : "Add sublocations";
        Caption = EntityNameForCaption;
        Advice = isConfirm
            ? $"Select a sublocation to amend the organisations in this {ProcessType}"
            : $"Select a sublocation to add organisations to this {ProcessType}";
    }
}
