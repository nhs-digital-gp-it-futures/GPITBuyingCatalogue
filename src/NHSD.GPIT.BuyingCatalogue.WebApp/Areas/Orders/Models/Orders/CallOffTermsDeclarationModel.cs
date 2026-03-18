using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;

public class CallOffTermsDeclarationModel : NavBaseModel
{
    public CallOffTermsDeclarationModel()
    {
    }

    public CallOffTermsDeclarationModel(Order order, string callOffTermsUrl)
    {
        CallOffId = order.CallOffId;
        CallOffTermsUrl = callOffTermsUrl;
        DeclarationAccepted = order.AcceptedTermsAndConditions;
    }

    public CallOffId CallOffId { get; set; }

    public bool DeclarationAccepted { get; set; }

    public string CallOffTermsUrl { get; set; }

    public override string Title => "Declaration";

    public override string Caption => $"Order {CallOffId}";
}
