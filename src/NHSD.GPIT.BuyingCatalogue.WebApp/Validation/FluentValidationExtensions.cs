using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Internal;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    [ExcludeFromCodeCoverage(Justification = "Contains extension methods for FluentValidation's IRuleBuilderOptions")]
    public static class FluentValidationExtensions
    {
        public const string InvalidUrlPrefixErrorMessage = "Enter a prefix to the URL, either http or https";
        public const string InvalidUrlErrorMessage = "Enter a valid URL";

        internal const string PriceEmptyError = "Enter a price";
        internal const string PriceNegativeError = "Price cannot be negative";
        internal const string PriceGreaterThanDecimalPlacesError = "Price must be to a maximum of 4 decimal places";

        public static IRuleBuilderOptions<T, TProperty> OverridePropertyName<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            params Expression<Func<T, object>>[] expressions)
        {
            if (expressions == null) throw new ArgumentNullException(nameof(expressions));
            var propertyName = string.Join('|', expressions.Select(expr => expr.GetMember()?.Name));
            return rule.OverridePropertyName(propertyName);
        }

        public static IRuleBuilderOptions<T, string> IsNumeric<T>(this IRuleBuilderInitial<T, string> ruleBuilder, string propertyName)
        {
            const string vowels = "aeiou";
            var name = propertyName.ToLower();

            // This won't always be true, some words beginning with 'h' require the indefinite article 'an' (eg hour),
            // and some words beginning with vowels require 'a' (eg unit), so expand with exceptions should the need arise
            var article = vowels.Contains(name.First()) ? "an" : "a";

            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage($"Enter {article} {name}")
                .Must(x => int.TryParse(x, out _))
                .WithMessage($"{name.CapitaliseFirstLetter()} must be a number");
        }

        public static IRuleBuilderOptions<T, string> IsNumericAndNonZero<T>(this IRuleBuilderInitial<T, string> ruleBuilder, string propertyName)
            => ruleBuilder.IsNumeric(propertyName)
                .Must(x => int.Parse(x) > 0)
                .WithMessage($"{propertyName.ToLower().CapitaliseFirstLetter()} must be greater than zero");

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
                .WithMessage(InvalidUrlPrefixErrorMessage)
                .MustAsync((link, _) => urlValidator.IsValidUrl(link))
                .WithMessage(InvalidUrlErrorMessage);

        public static IRuleBuilderOptions<T, decimal?> IsValidPrice<T>(this IRuleBuilderInitial<T, decimal?> ruleBuilder)
            => ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(PriceEmptyError)
                .GreaterThanOrEqualTo(0)
                .WithMessage(PriceNegativeError)
                .Must(p => Regex.IsMatch(p.ToString(), @"^\d+.?\d{0,4}$"))
                .WithMessage(PriceGreaterThanDecimalPlacesError);

        private static bool BePrefixedCorrectly(string url) => url.StartsWith("http") || url.StartsWith("https");
    }
}
