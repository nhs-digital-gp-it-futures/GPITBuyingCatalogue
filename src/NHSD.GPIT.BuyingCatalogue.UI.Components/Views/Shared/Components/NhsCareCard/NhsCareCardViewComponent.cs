using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsCareCard
{
    public sealed class NhsCareCardViewComponent : ViewComponent
    {
        private const string NhsCareCardNonUrgent = "nhsuk-care-card--non-urgent";
        private const string NhsCareCardUrgent = "nhsuk-care-card--urgent";
        private const string NhsCareCardImmediate = "nhsuk-care-card--immediate";

        public enum CareCardType
        {
            NonUrgent = 0,
            Urgent = 1,
            Immediate = 2,
        }

        public async Task<IViewComponentResult> InvokeAsync(string title, List<string> listOptions, string footer, CareCardType displayType)
        {
            var model = new NhsCareCardModel
            {
                Title = title,
                ListOptions = listOptions,
                Footer = footer,
            };

            model.CareCardClass = displayType switch
            {
                CareCardType.NonUrgent => NhsCareCardNonUrgent,
                CareCardType.Urgent => NhsCareCardUrgent,
                CareCardType.Immediate => NhsCareCardImmediate,
                _ => throw new ArgumentException($"{nameof(model.CareCardClass)} has an incorrect value of {model.CareCardClass}"),
            };

            return await Task.FromResult(View("NhsCareCard", model));
        }
    }
}
