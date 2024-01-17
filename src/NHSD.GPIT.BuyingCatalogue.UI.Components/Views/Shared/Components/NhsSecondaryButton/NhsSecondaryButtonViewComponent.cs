﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSecondaryButton
{
    public sealed class NhsSecondaryButtonViewComponent : ViewComponent
    {
        private const string NhsSecondary = "nhsuk-button--secondary";
        private const string NhsDelete = "nhsuk-button--delete";
        private const string DefaultHref = "#";

        public enum ButtonType
        {
            Primary = 0,
            Secondary = 1,
            Delete = 2,
        }

        public async Task<IViewComponentResult> InvokeAsync(string text, ButtonType type, string url = null, string id = null)
        {
            var model = new NhsSecondaryButtonModel
            {
                Text = text,
                Url = !string.IsNullOrEmpty(url) ? url : DefaultHref,
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
