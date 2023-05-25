namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public class BuyerDashboardModel
{
    internal const string AccountManagerAdvice =
        "Create and manage orders, competitions, filters and user accounts for your organisation.";

    internal const string BuyerAdvice = "Create and manage orders, competitions and filters for your organisation.";

    public BuyerDashboardModel(
        string internalOrgId,
        string organisationName,
        bool isAccountManager)
    {
        InternalOrgId = internalOrgId;

        OrganisationName = organisationName;

        Advice = isAccountManager
            ? AccountManagerAdvice
            : BuyerAdvice;
    }

    public string InternalOrgId { get; set; }

    public string OrganisationName { get; set; }

    public string Advice { get; set; }
}
