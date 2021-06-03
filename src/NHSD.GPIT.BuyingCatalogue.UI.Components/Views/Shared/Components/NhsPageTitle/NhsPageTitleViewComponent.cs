using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.ViewComponents.PageTitle
{
    public sealed class NhsPageTitleViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string title, string titleAdvice, string titleAdditionalAdvice)
        {
           var model = new NhsPageTitleModel
           {
                Title = title,
                Advice = titleAdvice,
                AdditionalAdvice = titleAdditionalAdvice,
           };

           return await Task.FromResult(View("NhsPageTitle", model));
        }
    }
}
