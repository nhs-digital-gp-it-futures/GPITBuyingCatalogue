using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders
{
    public sealed class TimeInputModelBinder : IModelBinder
    {
        private const string ErrorMessage = "Field is not in the correct format";

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            var modelState = bindingContext.ModelState;
            var val = bindingContext.ValueProvider.GetValue(modelName);

            if (val == ValueProviderResult.None)
                return Task.CompletedTask;

            var correctFormat = DateTime.TryParseExact(
                val.FirstValue,
                "HH:mm",
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime parsedDateTime);

            if (!correctFormat)
            {
                modelState.AddModelError(modelName, $"{modelName} {ErrorMessage}");
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(parsedDateTime);

            return Task.CompletedTask;
        }
    }
}
