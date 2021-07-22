﻿using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class BuyerTestBase : TestBase
    {
        protected BuyerTestBase(LocalWebApplicationFactory factory, Type controller, string methodName, IDictionary<string, string> parameters)
            : base(factory, GenerateUrlFromMethod(controller, methodName, parameters))
        {
            BuyerLogin();
        }
    }
}
