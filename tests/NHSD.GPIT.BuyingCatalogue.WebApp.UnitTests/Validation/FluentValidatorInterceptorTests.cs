using System.Collections.Generic;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation
{
    public static class FluentValidatorInterceptorTests
    {
        [Theory]
        [MockAutoData]
        public static void BeforeValidation_ReturnsContext(
            ActionContext actionContext,
            IValidationContext validationContext,
            FluentValidatorInterceptor interceptor)
        {
            var actualValidationContext = interceptor.BeforeAspNetValidation(actionContext, validationContext);

            actualValidationContext.Should().BeEquivalentTo(validationContext);
        }

        [Theory]
        [MockAutoData]
        public static void AfterValidation_NoErrors_ReturnsResult(
            ActionContext actionContext,
            IValidationContext validationContext,
            ValidationResult validationResult,
            FluentValidatorInterceptor interceptor)
        {
            validationResult.Errors.Clear();

            var result = interceptor.AfterAspNetValidation(actionContext, validationContext, validationResult);

            result.Should().BeEquivalentTo(validationResult);
        }

        [Theory]
        [MockAutoData]
        public static void AfterValidation_NoDelimitedErrors_ReturnsResult(
            ActionContext actionContext,
            IValidationContext validationContext,
            ValidationResult validationResult,
            FluentValidatorInterceptor interceptor)
        {
            validationResult.Errors.Clear();
            validationResult.Errors.Add(new ValidationFailure("SomeProperty", "some-error"));

            var result = interceptor.AfterAspNetValidation(actionContext, validationContext, validationResult);

            result.Should().BeEquivalentTo(validationResult);
        }

        [Theory]
        [MockAutoData]
        public static void AfterValidation_DelimitedErrors_AddsModelErrorForEach(
            ActionContext actionContext,
            IValidationContext validationContext,
            ValidationResult validationResult,
            FluentValidatorInterceptor interceptor)
        {
            var expectedValidationErrors = new List<ValidationFailure>
            {
                new("SomeProperty1", "some-error"),
                new("SomeProperty2", "some-error"),
            };

            validationResult.Errors.Clear();
            validationResult.Errors.Add(new ValidationFailure("SomeProperty1|SomeProperty2", "some-error"));

            var result = interceptor.AfterAspNetValidation(actionContext, validationContext, validationResult);

            result.Errors.Should().HaveCount(2);
            result.Errors.Should().BeEquivalentTo(expectedValidationErrors);
        }

        [Theory]
        [MockAutoData]
        public static void AfterValidation_DelimitedErrors_PreservesNonDelimitedErrors(
            ActionContext actionContext,
            IValidationContext validationContext,
            ValidationResult validationResult,
            FluentValidatorInterceptor interceptor)
        {
            var expectedValidationErrors = new List<ValidationFailure>
            {
                new("SomeProperty1", "some-error1"),
                new("SomeProperty2", "some-error1"),
                new("SomeProperty3", "some-error3"),
            };

            validationResult.Errors.Clear();
            validationResult.Errors.Add(new ValidationFailure("SomeProperty1|SomeProperty2", "some-error1"));
            validationResult.Errors.Add(new ValidationFailure("SomeProperty3", "some-error3"));

            var result = interceptor.AfterAspNetValidation(actionContext, validationContext, validationResult);

            result.Errors.Should().HaveCount(3);
            result.Errors.Should().BeEquivalentTo(expectedValidationErrors);
        }

        [Theory]
        [MockAutoData]
        public static void AfterValidation_DuplicatedProperties_DeduplicatesErrors(
            ActionContext actionContext,
            IValidationContext validationContext,
            ValidationResult validationResult,
            FluentValidatorInterceptor interceptor)
        {
            var expectedValidationErrors = new List<ValidationFailure>
            {
                new("SomeProperty3", "some-error3"),
            };

            validationResult.Errors.Clear();
            validationResult.Errors.Add(new("SomeProperty1", "some-error1"));
            validationResult.Errors.Add(new("SomeProperty3", "some-error3"));

            actionContext.ModelState.Clear();
            actionContext.ModelState.AddModelError("SomeProperty1", "MVC Error");

            var result = interceptor.AfterAspNetValidation(actionContext, validationContext, validationResult);

            result.Errors.Should().HaveCount(1);
            result.Errors.Should().BeEquivalentTo(expectedValidationErrors);
        }

        [Theory]
        [MockAutoData]
        public static void AfterValidation_FormattedMessagePlaceholderValues(
            ActionContext actionContext,
            IValidationContext validationContext,
            ValidationResult validationResult,
            FluentValidatorInterceptor interceptor)
        {
            var formattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                ["some-placeholder"] = "some-value",
            };
            var validationFailure = new ValidationFailure("SomeProperty", "some-error")
            {
                FormattedMessagePlaceholderValues = formattedMessagePlaceholderValues,
            };

            validationResult.Errors.Clear();
            validationResult.Errors.Add(validationFailure);

            var result = interceptor.AfterAspNetValidation(actionContext, validationContext, validationResult);

            result.Errors.Should().HaveCount(1);
            result.Errors[0].FormattedMessagePlaceholderValues.Should().BeEquivalentTo(formattedMessagePlaceholderValues);
        }
    }
}
