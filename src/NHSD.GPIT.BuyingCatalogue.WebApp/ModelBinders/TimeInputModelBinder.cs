using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders
{
    public sealed class TimeInputModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentException($"{nameof(modelName)} was found to be null in TimeInputModelBinder");

            var modelState = bindingContext.ModelState;

            if (modelState is null)
                throw new ArgumentNullException(nameof(modelState));

            var val = bindingContext.ValueProvider.GetValue(modelName);

            if (val == ValueProviderResult.None || string.IsNullOrWhiteSpace(val.FirstValue))
                return Task.CompletedTask;

            var correctFormat = DateTime.TryParseExact(
                val.FirstValue,
                "HH:mm",
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime parsedDateTime);

            if (!correctFormat)
            {
                modelState.AddModelError(modelName, "Enter time in the correct format");
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(parsedDateTime);

            return Task.CompletedTask;
        }
    }
}
