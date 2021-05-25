using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    public static class TagHelperFunctions
    {
        public static int? GetMaximumCharacterLength(ModelExpression aspFor)
        {
            return GetCustomAttribute<StringLengthAttribute>(aspFor)?.MaximumLength;
        }

        public static T GetCustomAttribute<T>(ModelExpression aspFor)
            where T : Attribute
        {
            return aspFor?.Metadata?
            .ContainerType?
            .GetProperty(aspFor.Name[(aspFor.Name.LastIndexOf('.') + 1)..])?
            .GetCustomAttribute<T>();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(ModelExpression aspFor)
            where T : Attribute
        {
            return aspFor?.Metadata?
            .ContainerType?
            .GetProperty(aspFor.Name[(aspFor.Name.LastIndexOf('.') + 1)..])?
            .GetCustomAttributes<T>();
        }

        public static bool IsCounterDisabled(ModelExpression aspFor, bool? htmlAttributeHideCharacterCounter)
        {
            return htmlAttributeHideCharacterCounter == true
                   || GetCustomAttribute<PasswordAttribute>(aspFor) != null;
        }

        public static bool CheckIfModelStateHasErrors(ViewContext viewContext, ModelExpression aspFor, string validationName = null)
        {
            var modelState = viewContext.ViewData?.ModelState;
            return !(modelState?[aspFor?.Name ?? validationName] is null) && modelState[aspFor?.Name ?? validationName].Errors.Any();
        }
    }
}
