using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ModelBinders
{
    public static class TimeInputModelBinderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static Task TimeInputModelBinder_NullOrWhitespaceModelName_ThrowsException(string modelName)
        {
            Mock<IValueProvider> valueProviderMock = new Mock<IValueProvider>();
            Mock<ModelBindingContext> contextMock = new Mock<ModelBindingContext>();
            TimeInputModelBinder modelBinder = new TimeInputModelBinder();

            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(ValueProviderResult.None);

            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ModelName).Returns(modelName);
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);

            return Assert.ThrowsAsync<ArgumentException>(() => modelBinder.BindModelAsync(contextMock.Object));
        }

        [Fact]
        public static async Task TimeInputModelBinder_NoValue_ResultIsFailed()
        {
            Mock<IValueProvider> valueProviderMock = new Mock<IValueProvider>();
            Mock<ModelBindingContext> contextMock = new Mock<ModelBindingContext>();
            TimeInputModelBinder modelBinder = new TimeInputModelBinder();

            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(ValueProviderResult.None);

            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ModelName).Returns("model");
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            contextMock.Setup(c => c.ModelState).Returns(new ModelStateDictionary());

            var context = contextMock.Object;

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(ModelBindingResult.Failed());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("1258")]
        [InlineData("25:01")]
        [InlineData("13:93")]
        [InlineData("12")]
        [InlineData("Hello")]
        public static async Task TimeInputModelBinder_ValueNotDateTimeConvertable_ResultsIsFailed(string value)
        {
            Mock<IValueProvider> valueProviderMock = new Mock<IValueProvider>();
            Mock<ModelBindingContext> contextMock = new Mock<ModelBindingContext>();
            TimeInputModelBinder modelBinder = new TimeInputModelBinder();

            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(new ValueProviderResult(value));

            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ModelName).Returns("model");
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            contextMock.Setup(c => c.ModelState).Returns(new ModelStateDictionary());

            var context = contextMock.Object;

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(ModelBindingResult.Failed());
        }

        [Fact]
        public static async Task TimeInputModelBinder_ReturnsSuccessWithDateTime()
        {
            var correctValue = "12:58";

            DateTime.TryParseExact(
                correctValue,
                "HH:mm",
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime parsedDateTime);

            var expectedValue = ModelBindingResult.Success(parsedDateTime);

            Mock<IValueProvider> valueProviderMock = new Mock<IValueProvider>();
            Mock<ModelBindingContext> contextMock = new Mock<ModelBindingContext>();
            TimeInputModelBinder modelBinder = new TimeInputModelBinder();

            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(new ValueProviderResult(correctValue));

            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ModelName).Returns("model");
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            contextMock.Setup(c => c.ModelState).Returns(new ModelStateDictionary());

            var context = contextMock.Object;

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(expectedValue);
            context.ModelState.IsValid.Should().BeTrue();
        }
    }
}
