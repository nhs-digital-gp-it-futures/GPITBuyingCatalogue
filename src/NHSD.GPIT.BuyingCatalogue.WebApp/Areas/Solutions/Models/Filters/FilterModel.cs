using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public abstract class FilterModel<T, T2>
    {
        public List<T2> Groups { get; set; }

        public SelectionModel[] SelectedItems { get; set; }

        public Dictionary<int, List<T>> GroupedItems { get; set; } = new();

        public List<T> Items(int groupId) => GroupedItems.ContainsKey(groupId)
            ? GroupedItems[groupId]
            : new List<T>();
    }
}
