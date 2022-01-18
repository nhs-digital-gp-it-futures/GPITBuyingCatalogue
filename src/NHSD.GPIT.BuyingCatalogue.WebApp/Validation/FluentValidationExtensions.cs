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

        public static IRuleBuilderOptions<T, string> IsValidQuantity<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
            => ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Enter a quantity")
                .Must(quantity => int.TryParse(quantity, out _))
                .WithMessage("Quantity must be a number")
                .Must(quantity => int.Parse(quantity) > 0)
                .WithMessage("Quantity must be greater than zero");

        public static IRuleBuilderOptions<T, int?> IsValidQuantity<T>(this IRuleBuilderInitial<T, int?> ruleBuilder)
            => ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Enter a quantity")
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");

        public static IRuleBuilderOptions<T, string> IsValidUrl<T>(this IRuleBuilderInitial<T, string> ruleBuilder, IUrlValidator urlValidator)
            => ruleBuilder
                .Cascade(CascadeMode.Stop)
                .Must(BePrefixedCorrectly)
                .WithMessage("Enter a prefix to the URL, either http or https")
                .MustAsync((link, _) => urlValidator.IsValidUrl(link))
                .WithMessage("Enter a valid URL");

        private static bool BePrefixedCorrectly(string url) => url.StartsWith("http") || url.StartsWith("https");
    }
}
