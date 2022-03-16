using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class AnonymousTestBase : TestBase
    {
        protected AnonymousTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            ITestOutputHelper testOutputHelper = null)
            : base(
                  factory,
                  testOutputHelper,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters))
        {
        }
    }
}
