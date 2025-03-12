using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.FormContent
{
    public class FilteredFormContentBinder : IModelBinder
    {
        private const string RequestVerificationTokenKey = "__RequestVerificationToken";

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var formData = bindingContext.HttpContext.Request.Form;
            var filteredFormData = formData.Where(kvp => kvp.Key != RequestVerificationTokenKey)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            bindingContext.Result = ModelBindingResult.Success(filteredFormData);
            return Task.CompletedTask;
        }
    }

}
