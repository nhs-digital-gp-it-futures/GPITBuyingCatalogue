namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public class BuyerDashboardModel
{
    internal const string AccountManagerAdvice =
        "Create and manage orders, competitions, filters and user accounts for your organisation.";

    internal const string BuyerAdvice = "Create and manage orders, competitions and filters for your organisation.";

    public BuyerDashboardModel(
        string organisationName,
        bool isAccountManager)
    {
        OrganisationName = organisationName;

        Advice = isAccountManager
            ? AccountManagerAdvice
            : BuyerAdvice;
    }

    public string OrganisationName { get; set; }

    public string Advice { get; set; }
}
