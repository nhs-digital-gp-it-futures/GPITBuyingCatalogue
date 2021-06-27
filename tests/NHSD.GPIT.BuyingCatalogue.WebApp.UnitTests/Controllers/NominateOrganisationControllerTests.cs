﻿using System;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class NominateOrganisationControllerTests
    {
        [Fact]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new NominateOrganisationController();

            var result = controller.Index();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }
    }
}
