using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.PageTitle
{
    public sealed class NhsPageTitleViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string title, string titleCaption, string titleAdvice, string titleAdditionalAdvice)
        {
           var model = new NhsPageTitleModel
           {
                Title = title,
                Caption = titleCaption,
                Advice = titleAdvice,
                AdditionalAdvice = titleAdditionalAdvice,
           };

           return await Task.FromResult(View("NhsPageTitle", model));
        }
    }
}
