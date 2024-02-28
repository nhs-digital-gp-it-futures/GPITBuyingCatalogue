using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsModalSearch
{
    public sealed class NhsModalSearchViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(
            string id,
            string showDialogButtonId,
            string callbackFunction,
            string title,
            string advice,
            string placeholder,
            string notFoundText,
            string applyButtonText,
            string tableContentFunction = null,
            string tablePartialView = null,
            object tableData = null,
            bool clearSearch = true,
            bool clearSelection = true)
        {
            if (string.IsNullOrEmpty(id) || id.Any(char.IsWhiteSpace))
                throw new ArgumentException("Id cannot be null or empty, or contain whitespace", nameof(id));

            var model = new NhsModalSearchModel
            {
                Id = id,
                ShowDialogButton = showDialogButtonId,
                TablePartialView = tablePartialView,
                TableData = tableData,
                CallbackFunction = callbackFunction,
                Title = title,
                Placeholder = placeholder,
                Advice = advice,
                NotFoundText = notFoundText,
                ApplyButtonText = applyButtonText,
                TableContentFunction = tableContentFunction,
                ClearSearch = clearSearch,
                ClearSelection = clearSelection,
            };

            return await Task.FromResult(View("NhsModalSearch", model));
        }
    }
}
