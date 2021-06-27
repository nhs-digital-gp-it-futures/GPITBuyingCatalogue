﻿using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class GuideControllerTests
    {
        [Fact]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new GuideController();

            var result = controller.Index();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }
    }
}
