using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSecondaryButton
{
    public sealed class NhsSecondaryButtonViewComponent : ViewComponent
    {
        private const string NhsSecondary = "nhsuk-button--secondary";
        private const string NhsDelete = "nhsuk-button--delete";

        public enum ButtonType
        {
            Primary = 0,
            Secondary = 1,
            Delete = 2,
        }

        public async Task<IViewComponentResult> InvokeAsync(string text, string url, ButtonType type, string id = null)
        {
            var model = new NhsSecondaryButtonModel
            {
                Text = text,
                Url = url,
                Id = id,
            };

            model.ButtonClass = type switch
            {
                ButtonType.Primary => string.Empty,
                ButtonType.Secondary => NhsSecondary,
                ButtonType.Delete => NhsDelete,
                _ => throw new ArgumentException($"{nameof(model.ButtonClass)} has an incorrect value of {model.ButtonClass}"),
            };

            return await Task.FromResult(View("NhsSecondaryButton", model));
        }
    }
}
