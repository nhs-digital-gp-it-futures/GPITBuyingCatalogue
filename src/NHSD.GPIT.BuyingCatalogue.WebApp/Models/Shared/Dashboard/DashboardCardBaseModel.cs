using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Dashboard;

public class DashboardCardBaseModel<T>
{
    public DashboardCardBaseModel(
        string internalOrgId,
        ICollection<T> items,
        int numberOfItems,
        bool isDashboardView = false,
        PageOptions pageOptions = null)
    {
        InternalOrgId = internalOrgId;
        Items = items;
        NumberOfItems = numberOfItems;
        IsDashboardView = isDashboardView;
        PageOptions = pageOptions;
    }

    public string InternalOrgId { get; set; }

    public PageOptions PageOptions { get; set; }

    public ICollection<T> Items { get; set; }

    public int NumberOfItems { get; set; }

    public bool ShouldUsePagination => PageOptions is not null;

    public bool IsDashboardView { get; set; }
}
