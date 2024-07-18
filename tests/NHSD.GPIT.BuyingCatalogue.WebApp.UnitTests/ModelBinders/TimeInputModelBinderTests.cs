using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ModelBinders
{
    public static class TimeInputModelBinderTests
    {
        [Theory]
        [MockInlineAutoData("")]
        [MockInlineAutoData("\t")]
        public static Task TimeInputModelBinder_EmptyOrWhitespaceModelName_ThrowsException(
            string modelName,
            DefaultModelBindingContext context,
            TimeInputModelBinder modelBinder)
        {
            context.ModelName = modelName;
            context.ValueProvider.GetValue(Arg.Any<string>()).Returns(ValueProviderResult.None);

            return Assert.ThrowsAsync<ArgumentException>(() => modelBinder.BindModelAsync(context));
        }

        [Theory]
        [MockAutoData]
        public static async Task TimeInputModelBinder_NoValue_ResultIsFailed(
            DefaultModelBindingContext context,
            TimeInputModelBinder modelBinder)
        {
            context.ModelName = "model";
            context.ModelState = new ModelStateDictionary();
            context.ValueProvider.GetValue(Arg.Any<string>()).Returns(ValueProviderResult.None);

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(ModelBindingResult.Failed());
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("\t")]
        [MockInlineAutoData("1258")]
        [MockInlineAutoData("25:01")]
        [MockInlineAutoData("13:93")]
        [MockInlineAutoData("12")]
        [MockInlineAutoData("Hello")]
        public static async Task TimeInputModelBinder_ValueNotDateTimeConvertable_ResultsIsFailed(
            string value,
            DefaultModelBindingContext context,
            TimeInputModelBinder modelBinder)
        {
            context.ModelName = "model";
            context.ModelState = new ModelStateDictionary();
            context.ValueProvider.GetValue(Arg.Any<string>()).Returns(new ValueProviderResult(value));

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(ModelBindingResult.Failed());
        }

        [Theory]
        [MockAutoData]
        public static async Task TimeInputModelBinder_ReturnsSuccessWithDateTime(
            DefaultModelBindingContext context,
            TimeInputModelBinder modelBinder)
        {
            const string correctValue = "12:58";

            DateTime.TryParseExact(
                correctValue,
                "HH:mm",
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime parsedDateTime);

            var expectedValue = ModelBindingResult.Success(parsedDateTime);

            context.ModelName = "model";
            context.ModelState = new ModelStateDictionary();
            context.ValueProvider.GetValue(Arg.Any<string>()).Returns(new ValueProviderResult(correctValue));

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(expectedValue);
            context.ModelState.IsValid.Should().BeTrue();
        }
    }
}
