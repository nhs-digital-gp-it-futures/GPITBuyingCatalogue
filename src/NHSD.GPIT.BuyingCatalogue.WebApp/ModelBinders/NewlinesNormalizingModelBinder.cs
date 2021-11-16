using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders
{
    public sealed class NewlinesNormalizingModelBinder : IModelBinder
    {
        private readonly IModelBinder modelBinder;

        public NewlinesNormalizingModelBinder(IModelBinder modelBinder)
        {
            this.modelBinder = modelBinder ?? throw new ArgumentNullException(nameof(modelBinder));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            if (string.IsNullOrWhiteSpace(bindingContext.ModelName))
                throw new ArgumentException($"{nameof(bindingContext.ModelName)} was found to be null in NewlinesNormalizingModelBinder");

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None && valueProviderResult.FirstValue is string str && !string.IsNullOrEmpty(str))
            {
                bindingContext.Result = ModelBindingResult.Success(str.Replace("\r\n", "\n"));
                return Task.CompletedTask;
            }

            return modelBinder.BindModelAsync(bindingContext);
        }
    }
}
