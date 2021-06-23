using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Microsoft.Extensions.Primitives;
using System;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class ArgumentNullActionFilterTests
    {
        public static IEnumerable<TestCaseData> FailingCases
        {
            get
            {
                yield return new TestCaseData(null, Guid.Parse("54a18b28-8491-467c-bf0f-90f29660bd16"), new object())
                    .SetName("ArgumentNullActionFilter_ParamsNullOrEmpty_ExpectBadRequest_aStringValueIsNull");

                yield return new TestCaseData(string.Empty, Guid.Parse("54a18b28-8491-467c-bf0f-90f29660bd16"), new object())
                    .SetName("ArgumentNullActionFilter_ParamsNullOrEmpty_ExpectBadRequest_aStringValueIsEmpty");

                yield return new TestCaseData("Hello,World", Guid.Empty, new object())
                    .SetName("ArgumentNullActionFilter_ParamsNullOrEmpty_ExpectBadRequest_aGuidValueIsEmpty");

                yield return new TestCaseData("Hello,World", null, new object())
                    .SetName("ArgumentNullActionFilter_ParamsNullOrEmpty_ExpectBadRequest_aGuidValueIsNull");

                yield return new TestCaseData("Hello,World", Guid.Parse("54a18b28-8491-467c-bf0f-90f29660bd16"), null)
                    .SetName("ArgumentNullActionFilter_ParamsNullOrEmpty_ExpectBadRequest_anObjectValueIsNull");

                yield return new TestCaseData(null, null, null)
                    .SetName("ArgumentNullActionFilter_ParamsNullOrEmpty_ExpectBadRequest_AllValuesAreNull");
            }
        }

        [Test]  
        public async Task ArgumentNullActionFilter_ParamsNotNull_ExpectSuccess()
        {
            var modelState = new ModelStateDictionary();

            var httpContextMock = new DefaultHttpContext();

            var actionContext = new ActionContext(
                httpContextMock,
                Mock.Of<Microsoft.AspNetCore.Routing.RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            )
            {
                Result = new OkResult() // It will return ok unless during code execution you change this when by condition
            };

            var ListOfLogStrings = new List<string>();

            var mockLogger = new Mock<ILogWrapper<ActionArgumentNullFilter>>();

            mockLogger
                .Setup(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((l, _) => ListOfLogStrings.Add(l));

            actionExecutingContext.ActionArguments.Add("AStringValue", "Hello,World");
            actionExecutingContext.ActionArguments.Add("AGuidValue", Guid.Parse("54a18b28-8491-467c-bf0f-90f29660bd16"));
            actionExecutingContext.ActionArguments.Add("AnObjectValue", new object());

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var ActionArgumentFilter = new ActionArgumentNullFilter(mockLogger.Object);

            await ActionArgumentFilter.OnActionExecutionAsync(actionExecutingContext, async () => { return await Task.FromResult(context); });

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
            ListOfLogStrings.Count.Should().Be(0);
        }

        [TestCaseSource(nameof(FailingCases))]
        public async Task ArgumentNullActionFilter_ParamsNullOrEmpty_ExpectBadRequest(string AStringValue, Guid AGuidValue, object AnObjectValue)
        {
            var modelState = new ModelStateDictionary();

            var httpContextMock = new DefaultHttpContext();

            var actionContext = new ActionContext(
                httpContextMock,
                Mock.Of<Microsoft.AspNetCore.Routing.RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            )
            {
                Result = new OkResult() // It will return ok unless during code execution you change this when by condition
            };

            var ListOfLogStrings = new List<string>();

            var mockLogger = new Mock<ILogWrapper<ActionArgumentNullFilter>>();

            mockLogger
                .Setup(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string,object[]>((l, _) => ListOfLogStrings.Add(l));

            actionExecutingContext.ActionArguments.Add("AStringValue", AStringValue);
            actionExecutingContext.ActionArguments.Add("AGuidValue", AGuidValue);
            actionExecutingContext.ActionArguments.Add("AnObjectValue", AnObjectValue);

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var ActionArgumentFilter = new ActionArgumentNullFilter(mockLogger.Object);

            await ActionArgumentFilter.OnActionExecutionAsync(actionExecutingContext, async () => { return await Task.FromResult(context); });

            actionExecutingContext.Result.Should().BeOfType<BadRequestResult>();
            ListOfLogStrings.Count.Should().Be(1);
        }
    }
}
