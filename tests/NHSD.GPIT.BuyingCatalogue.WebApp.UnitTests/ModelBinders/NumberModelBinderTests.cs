﻿using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ModelBinders;

public static class NumberModelBinderTests
{
    [Theory]
    [MockAutoData]
    public static async Task BindModelAsync_NullValue_DoesNothing(
        DefaultModelBindingContext context,
        NumberModelBinder binder)
    {
        context.Result = default;

        context.ValueProvider.GetValue(context.ModelName)
            .Returns(ValueProviderResult.None);

        await binder.BindModelAsync(context);

        context.Result.Should().Be(ModelBindingResult.Failed());
    }

    [Theory]
    [MockAutoData]
    public static async Task BindModelAsync_ValidInteger_SetsSuccessResult(
        string modelName,
        int value,
        DefaultModelBindingContext context,
        NumberModelBinder binder)
    {
        context.ModelName = modelName;
        context.Result = default;

        var stringValues = new StringValues(value.ToString());

        var result = new ValueProviderResult(stringValues);

        context.ValueProvider.GetValue(Arg.Any<string>())
            .Returns(result);

        await binder.BindModelAsync(context);

        context.Result.Should().Be(ModelBindingResult.Success(value));
    }
}
