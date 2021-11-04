using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSubmitButton
{
    public sealed class NhsSubmitButtonViewComponent : ViewComponent
    {
        private const string DefaultButtonText = "Save and continue";

        public async Task<IViewComponentResult> InvokeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                text = DefaultButtonText;

            return await Task.FromResult(View("NhsSubmitButton", text));
        }
    }
}
