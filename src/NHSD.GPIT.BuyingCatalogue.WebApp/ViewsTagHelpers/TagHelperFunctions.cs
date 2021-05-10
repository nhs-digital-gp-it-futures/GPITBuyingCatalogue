using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    public static class TagHelperFunctions
    {
        public static int? GetMaximumCharacterLength(ModelExpression For)
        {
            return GetCustomAttribute<StringLengthAttribute>(For)?.MaximumLength;
        }

        public static T GetCustomAttribute<T>(ModelExpression For) where T : Attribute
        {
            return For?.Metadata?
            .ContainerType?
            .GetProperty(For.Name)?
            .GetCustomAttribute<T>();
        }
        public static IEnumerable<T> GetCustomAttributes<T>(ModelExpression For) where T : Attribute
        {
            return For?.Metadata?
            .ContainerType?
            .GetProperty(For.Name)?
            .GetCustomAttributes<T>();
        }

        public static bool IsCounterDisabled(ModelExpression For, bool? HtmlAttributeHideCharacterCounter)
        {
            return GetCustomAttribute<DisableCharacterCounterAttribute>(For) != null
                   || HtmlAttributeHideCharacterCounter == true
                   || GetCustomAttribute<PasswordAttribute>(For) != null;
        }

        public static int GetTextAreaNumberOfRows(ModelExpression For, int? HtmlAttributeNumberOfRows)
        {
            var NumberOfRowsAttribute = GetCustomAttribute<TextAreaRowsAttribute>(For);

            return HtmlAttributeNumberOfRows ?? NumberOfRowsAttribute?.NumberOfRows ?? TagHelperConstants.DefaultNumberOfTextAreaRows;
        }

        public static bool CheckIfModelStateHasErrors(ViewContext viewContext, ModelExpression For, string validationName = null)
        {
            var modelState = viewContext.ViewData?.ModelState;
            return !(modelState?[For?.Name ?? validationName] is null) && modelState[For?.Name ?? validationName].Errors.Any();
        }
    }
}
