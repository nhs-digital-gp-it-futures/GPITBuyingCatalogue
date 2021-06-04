using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
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

        public static string GetModelKebabNameFromFor(ModelExpression aspFor)
        {
            string name = aspFor.Model.GetType().Name;

            // removes the word Model from the end of the Model, e.g SolutionDescriptionModel becomes SolutionDescription
            name = name.Remove(name.Length - 5);

            var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return string.Join("-", pattern.Matches(name)).ToLower();
        }
    }
}
