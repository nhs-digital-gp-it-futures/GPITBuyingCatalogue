using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;

public class NumberModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (value == ValueProviderResult.None || string.IsNullOrWhiteSpace(value.FirstValue))
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

        if (!int.TryParse(value.FirstValue, out var parsedValue))
        {
            var description = ((DefaultModelMetadata)bindingContext.ModelMetadata).Attributes.Attributes
                .OfType<DescriptionAttribute>()
                .FirstOrDefault();

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"{description?.Description} must be a whole number");
            bindingContext.Result = ModelBindingResult.Failed();

            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(parsedValue);
        return Task.CompletedTask;
    }
}
