using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public sealed class AddOrConfirmSublocationsModel : NavBaseModel
{
    public AddOrConfirmSublocationsModel(bool isConfirm, Competition competition, Organisation organisation)
    {
        Title = isConfirm ? "Confirm sublocations" : "Add sublocations";
        Caption = $"{competition.Name}";
        Advice = isConfirm
            ? "Select a sublocation to amend the organisations in this competition"
            : "Select a sublocation to add organisations to this competition";
    }

    public AddOrConfirmSublocationsModel(bool isConfirm, Order order, Organisation organisation)
    {
    }
}
