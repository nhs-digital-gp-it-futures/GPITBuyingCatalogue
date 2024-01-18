using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
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

        public async Task<IViewComponentResult> InvokeAsync(string id, string showDialogButtonId, string tablePartialView, object tableData, string title = null, string advice = null, string placeholder = null, string notFoundText = null, string applyButtonText = null)
        {
            var model = new NhsModalSearchModel
            {
                Id = id,
                ShowDialogButton = showDialogButtonId,
                TablePartialView = tablePartialView,
                TableData = tableData,
                Title = !string.IsNullOrEmpty(title) ? title : DefaultTitle,
                Placeholder = !string.IsNullOrEmpty(placeholder) ? placeholder : DefaultPlaceholder,
                Advice = !string.IsNullOrEmpty(advice) ? advice : DefaultAdvice,
                NotFoundText = !string.IsNullOrEmpty(notFoundText) ? notFoundText : DefaultNotFoundText,
                ApplyButtonText = !string.IsNullOrEmpty(applyButtonText) ? applyButtonText : DefaultApplyButtonText,
            };

            return await Task.FromResult(View("NhsModalSearch", model));
        }
    }
}
