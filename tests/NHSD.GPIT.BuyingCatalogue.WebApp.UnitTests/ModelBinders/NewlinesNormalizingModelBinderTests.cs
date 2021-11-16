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
        [InlineData("Abc", 3)]
        [InlineData("Abc\ndef", 7)]
        [InlineData("Abc\rdef", 7)]
        [InlineData("Abc\r\ndef", 7)]
        [InlineData("Abc\rdef\nghi", 11)]
        public static void ValidInput_CorrectlyRemovesCarriageReturn(string input, int expectedCount)
        {
            var valueProviderMock = new Mock<IValueProvider>();
            var contextMock = new Mock<ModelBindingContext>();
            var modelStateMock = new Mock<ModelStateDictionary>();

            var modelBinder = new NewlinesNormalizingModelBinder();

            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(new ValueProviderResult(input));

            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ModelName).Returns("Description");
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            contextMock.Setup(c => c.ModelState).Returns(modelStateMock.Object);

            modelBinder.BindModelAsync(contextMock.Object);

            contextMock.Object.Result.Model.As<string>().Length.Should().Be(expectedCount);
        }
    }
}
