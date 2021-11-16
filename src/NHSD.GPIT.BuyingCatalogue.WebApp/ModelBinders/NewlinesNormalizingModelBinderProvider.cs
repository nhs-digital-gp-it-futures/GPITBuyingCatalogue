using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders
{
    public class NewlinesNormalizingModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(string))
                return new NewlinesNormalizingModelBinder();

            return null;
        }
    }
}
