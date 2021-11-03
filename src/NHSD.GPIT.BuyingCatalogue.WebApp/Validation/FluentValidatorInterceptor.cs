using System.Linq;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    public class FluentValidatorInterceptor : IValidatorInterceptor
    {
        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            if (!result.Errors.Any())
                return result;

            var delimitedErrors = result.Errors.Where(e => e.PropertyName.Contains('|')).ToList();
            if (!delimitedErrors.Any())
                return result;

            var validationErrors = result.Errors.Except(delimitedErrors).ToList();
            foreach (var delimitedError in delimitedErrors)
            {
                foreach (var propertyName in delimitedError.PropertyName.Split('|'))
                {
                    var validationError = new ValidationFailure(propertyName, delimitedError.ErrorMessage)
                    {
                        AttemptedValue = delimitedError.AttemptedValue,
                        CustomState = delimitedError.CustomState,
                        ErrorCode = delimitedError.ErrorCode,
                        FormattedMessagePlaceholderValues = delimitedError.FormattedMessagePlaceholderValues?.ToDictionary(k => k.Key, v => v.Value),
                        Severity = delimitedError.Severity,
                    };

                    validationErrors.Add(validationError);
                }
            }

            result.Errors.Clear();
            result.Errors.AddRange(validationErrors);

            return result;
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
            => commonContext;
    }
}
