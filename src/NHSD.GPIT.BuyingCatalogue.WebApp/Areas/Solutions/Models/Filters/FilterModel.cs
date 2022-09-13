using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public abstract class FilterModel<T, T2>
    {
        public IEnumerable<T2> Groups { get; set; }

        public SelectionModel[] SelectedItems { get; set; }

        public int Total { get; set; }

        public string SearchTerm { get; set; }

        protected Dictionary<int, IOrderedEnumerable<T>> GroupedItems { get; set; } = new();

        public List<T> Items(int groupId) => GroupedItems.ContainsKey(groupId)
            ? GroupedItems[groupId].ToList()
            : new List<T>();
    }
}
