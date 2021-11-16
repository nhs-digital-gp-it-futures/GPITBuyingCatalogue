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
        public static void Constructor_NullModelBinder_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new NewlinesNormalizingModelBinder(null));
        }

        [Fact]
        public static Task BindModelAsync_NullBindingContext_ThrowsException()
        {
            var binderMock = new Mock<IModelBinder>();
            var modelBinder = new NewlinesNormalizingModelBinder(binderMock.Object);

            return Assert.ThrowsAsync<ArgumentException>(() => modelBinder.BindModelAsync(null));            
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static Task NullOrWhitespaceModelName_ThrowsException(string modelName)
        {
            var valueProviderMock = new Mock<IValueProvider>();
            var contextMock = new Mock<ModelBindingContext>();
            var binderMock = new Mock<IModelBinder>();
            var modelBinder = new NewlinesNormalizingModelBinder(binderMock.Object);

            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(ValueProviderResult.None);

            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ModelName).Returns(modelName);
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);

            return Assert.ThrowsAsync<ArgumentException>(() => modelBinder.BindModelAsync(contextMock.Object));
        }

        [Theory]
        [InlineData("Abc", 3)]
        [InlineData("Abc\ndef", 7)]
        [InlineData("Abc\rdef", 7)]
        [InlineData("Abc\r\ndef", 7)]
        [InlineData("Abc\rdef\nghi", 11)]
        public static void ValidInput_CorrectlyRemovesCarriageReturn(string input, int expectedCount)
        {
            Mock<IValueProvider> valueProviderMock = new Mock<IValueProvider>();
            Mock<ModelBindingContext> contextMock = new Mock<ModelBindingContext>();
            var binderMock = new Mock<IModelBinder>();
            var modelBinder = new NewlinesNormalizingModelBinder(binderMock.Object);

            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(new ValueProviderResult(input));

            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ModelName).Returns("Description");
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);

            modelBinder.BindModelAsync(contextMock.Object);

            contextMock.Object.Result.Model.As<string>().Length.Should().Be(expectedCount);
        }
    }
}
