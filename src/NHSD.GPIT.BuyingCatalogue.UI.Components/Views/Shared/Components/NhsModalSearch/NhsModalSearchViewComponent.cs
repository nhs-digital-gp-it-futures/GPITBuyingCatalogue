using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsModalSearch
{
    public sealed class NhsModalSearchViewComponent : ViewComponent
    {
        private const string DefaultTitle = "Search for Service Recipients";
        private const string DefaultAdvice = "Find and select the Service Recipients you want to include in this order.";
        private const string DefaultPlaceholder = "Search by ODS code or organisation name";
        private const string DefaultNotFoundText = "There are no Service Recipients that match. Try entering different search criteria.";
        private const string DefaultApplyButtonText = "Apply recipients";

        public async Task<IViewComponentResult> InvokeAsync(string id, string showDialogButtonId, string tablePartialView, object tableData, string callbackFunction, string title = null, string advice = null, string placeholder = null, string notFoundText = null, string applyButtonText = null, bool cleardown = true)
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
                Title = !string.IsNullOrEmpty(title) ? title : DefaultTitle,
                Placeholder = !string.IsNullOrEmpty(placeholder) ? placeholder : DefaultPlaceholder,
                Advice = !string.IsNullOrEmpty(advice) ? advice : DefaultAdvice,
                NotFoundText = !string.IsNullOrEmpty(notFoundText) ? notFoundText : DefaultNotFoundText,
                ApplyButtonText = !string.IsNullOrEmpty(applyButtonText) ? applyButtonText : DefaultApplyButtonText,
                Cleardown = cleardown,
            };

            return await Task.FromResult(View("NhsModalSearch", model));
        }
    }
}
