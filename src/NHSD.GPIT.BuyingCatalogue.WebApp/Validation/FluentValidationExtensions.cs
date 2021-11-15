using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Internal;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    [ExcludeFromCodeCoverage(Justification = "Contains extension methods for FluentValidation's IRuleBuilderOptions")]
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> OverridePropertyName<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            params Expression<Func<T, object>>[] expressions)
        {
            if (expressions == null) throw new ArgumentNullException(nameof(expressions));
            var propertyName = string.Join('|', expressions.Select(expr => expr.GetMember()?.Name));
            return rule.OverridePropertyName(propertyName);
        }

        public static IRuleBuilderOptions<T, string> IsValidUrl<T>(this IRuleBuilder<T, string> ruleBuilder, IUrlValidator urlValidator)
        {
            return ruleBuilder
                .MustAsync((link, _) => urlValidator.IsValidUrl(link))
                .WithMessage("Enter a valid URL");
        }
    }
}
