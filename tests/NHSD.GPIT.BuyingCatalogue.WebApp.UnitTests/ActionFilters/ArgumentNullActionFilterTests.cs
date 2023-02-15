﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public static class ArgumentNullActionFilterTests
    {
        [Fact]
        public static async Task ArgumentNullActionFilter_ParamsNotNull_ExpectSuccess()
        {
            var modelState = new ModelStateDictionary();

            var httpContextMock = new DefaultHttpContext();

            var actionContext = new ActionContext(
                httpContextMock,
                Mock.Of<Microsoft.AspNetCore.Routing.RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState);

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>())
            {
                Result = new OkResult(), // It will return ok unless during code execution you change this when by condition
            };

            var listOfLogStrings = new List<string>();

            var mockLogger = new Mock<ILogWrapper<ActionArgumentNullFilter>>();

            mockLogger
                .Setup(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((l, _) => listOfLogStrings.Add(l));

            actionExecutingContext.ActionArguments.Add("AStringValue", "Hello,World");
            actionExecutingContext.ActionArguments.Add("AGuidValue", Guid.Parse("54a18b28-8491-467c-bf0f-90f29660bd16"));
            actionExecutingContext.ActionArguments.Add("AnObjectValue", new object());

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var actionArgumentFilter = new ActionArgumentNullFilter(mockLogger.Object);

            await actionArgumentFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(context));

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
            listOfLogStrings.Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(FailingCaseData.TestData), MemberType = typeof(FailingCaseData))]
        public static async Task ArgumentNullActionFilter_ParamsNullOrEmpty_ExpectBadRequest(string aStringValue, Guid aGuidValue, object anObjectValue)
        {
            var modelState = new ModelStateDictionary();

            var httpContextMock = new DefaultHttpContext();

            var actionContext = new ActionContext(
                httpContextMock,
                Mock.Of<Microsoft.AspNetCore.Routing.RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState);

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>())
            {
                Result = new OkResult(), // It will return ok unless during code execution you change this when by condition
            };

            var listOfLogStrings = new List<string>();

            var mockLogger = new Mock<ILogWrapper<ActionArgumentNullFilter>>();

            mockLogger
                .Setup(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((l, _) => listOfLogStrings.Add(l));

            actionExecutingContext.ActionArguments.Add("AStringValue", aStringValue);
            actionExecutingContext.ActionArguments.Add("AGuidValue", aGuidValue);
            actionExecutingContext.ActionArguments.Add("AnObjectValue", anObjectValue);

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var actionArgumentFilter = new ActionArgumentNullFilter(mockLogger.Object);

            await actionArgumentFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(context));

            actionExecutingContext.Result.Should().BeOfType<BadRequestResult>();
            listOfLogStrings.Count.Should().Be(1);
        }

        private static class FailingCaseData
        {
            public static IEnumerable<object[]> TestData()
            {
                yield return new[] { null, Guid.Parse("54a18b28-8491-467c-bf0f-90f29660bd16"), new object() };
                yield return new[] { string.Empty, Guid.Parse("54a18b28-8491-467c-bf0f-90f29660bd16"), new object() };
                yield return new[] { "Hello,World", Guid.Empty, new object() };
                yield return new[] { "Hello,World", null, new object() };
                yield return new object[] { "Hello,World", Guid.Parse("54a18b28-8491-467c-bf0f-90f29660bd16"), null };
                yield return new object[3];
            }
        }
    }
}
