using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public sealed class AddOrConfirmSublocationsModel : NavBaseModel
{
    public AddOrConfirmSublocationsModel(
        bool isConfirm,
        Competition competition,
        Organisation organisation,
        string addOrChangeSublocationsHref)
    {
        ProcessType = "competition";
        EntityNameForCaption = competition.Name;
        Organisation = organisation;

        SetTitleParams(isConfirm);

        AddOrChangeSublocationsHref = addOrChangeSublocationsHref;
    }

    public AddOrConfirmSublocationsModel(bool isConfirm, Order order, Organisation organisation)
    {
        ProcessType = "order";
        EntityNameForCaption = order.Description;
        Organisation = organisation;

        SetTitleParams(isConfirm);
    }

    public string ProcessType { get; set; }

    public string EntityNameForCaption { get; set; }

    public Organisation Organisation { get; set; }

    public string AddOrChangeSublocationsHref { get; set; }

    private void SetTitleParams(bool isConfirm)
    {
        Title = isConfirm ? "Confirm sublocations" : "Add sublocations";
        Caption = EntityNameForCaption;
        Advice = isConfirm
            ? $"Select a sublocation to amend the organisations in this {ProcessType}"
            : $"Select a sublocation to add organisations to this {ProcessType}";
    }
}
