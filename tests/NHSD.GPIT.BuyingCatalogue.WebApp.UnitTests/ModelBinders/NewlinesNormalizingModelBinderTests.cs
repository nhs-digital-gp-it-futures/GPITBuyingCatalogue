using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ModelBinders
{
    public static class NewlinesNormalizingModelBinderTests
    {
        [Fact]
        public static Task BindModelAsync_NullBindingContext_ThrowsException()
        {
            var modelBinder = new NewlinesNormalizingModelBinder();

            return Assert.ThrowsAsync<ArgumentNullException>(() => modelBinder.BindModelAsync(null));
        }

        [Theory]
        [MockInlineAutoData("Abc", 3)]
        [MockInlineAutoData("Abc\ndef", 7)]
        [MockInlineAutoData("Abc\rdef", 7)]
        [MockInlineAutoData("Abc\r\ndef", 7)]
        [MockInlineAutoData("Abc\rdef\nghi", 11)]
        public static void ValidInput_CorrectlyRemovesCarriageReturn(
            string input,
            int expectedCount,
            ModelStateDictionary modelState,
            DefaultModelBindingContext context,
            NewlinesNormalizingModelBinder modelBinder)
        {
            context.ModelName = "Description";
            context.ModelState = modelState;
            context.ValueProvider.GetValue(Arg.Any<string>()).Returns(new ValueProviderResult(input));

            modelBinder.BindModelAsync(context);

            context.Result.Model.As<string>().Length.Should().Be(expectedCount);
        }
    }
}
