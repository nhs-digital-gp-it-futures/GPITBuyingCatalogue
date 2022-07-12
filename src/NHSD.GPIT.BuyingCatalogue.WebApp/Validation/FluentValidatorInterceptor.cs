using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    public class FluentValidatorInterceptor : IValidatorInterceptor
    {
        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            if (!result.Errors.Any())
                return result;

            var errors = GetDistinctErrors(actionContext.ModelState, result.Errors).ToList();

            var delimitedErrors = errors.Where(e => e.PropertyName.Contains('|')).ToList();
            if (!delimitedErrors.Any())
            {
                result.Errors.Clear();
                result.Errors.AddRange(errors);
            }

            var validationErrors = errors.Except(delimitedErrors).ToList();
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

        private IEnumerable<ValidationFailure> GetDistinctErrors(ModelStateDictionary modelStateDictionary, List<ValidationFailure> failures)
        {
            var modelStateFailures = modelStateDictionary.Where(e => e.Value.ValidationState == ModelValidationState.Invalid).ToList();
            foreach (var failure in failures)
            {
                if (modelStateFailures.Any(e => string.Equals(e.Key, failure.PropertyName, System.StringComparison.OrdinalIgnoreCase)))
                    continue;

                yield return failure;
            }
        }
    }
}
