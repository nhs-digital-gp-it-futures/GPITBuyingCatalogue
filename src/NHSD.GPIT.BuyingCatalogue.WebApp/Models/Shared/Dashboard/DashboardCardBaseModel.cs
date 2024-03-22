using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Dashboard;

public class DashboardCardBaseModel<T>
{
    public DashboardCardBaseModel(
        string internalOrgId,
        ICollection<T> items,
        bool isDashboardView = false,
        PageOptions pageOptions = null)
    {
        InternalOrgId = internalOrgId;
        Items = items;
        IsDashboardView = isDashboardView;
        PageOptions = pageOptions;
    }

    public string InternalOrgId { get; set; }

    public PageOptions PageOptions { get; set; }

    public ICollection<T> Items { get; set; }

    public int NumberOfItems => Items.Count;

    public bool ShouldUsePagination => PageOptions is not null;

    public bool IsDashboardView { get; set; }

    public bool HasItems => NumberOfItems > 0;

    public int ItemsToIterate => IsDashboardView ? 5 : NumberOfItems;
}
