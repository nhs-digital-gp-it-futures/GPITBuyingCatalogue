using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType

{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ClientApplicationTypesModelTests
    {
        [Test]
        public static void IsComplete_BrowserBasedIsTrue_ReturnsTrue()
        {
            var model = new ClientApplicationTypesModel{ BrowserBased = true, };

            model.IsComplete.Should().BeTrue();
        }
        
        [Test]
        public static void IsComplete_NativeDesktopIsTrue_ReturnsTrue()
        {
            var model = new ClientApplicationTypesModel{ NativeDesktop = true, };

            model.IsComplete.Should().BeTrue();
        }

        [Test]
        public static void IsComplete_NativeMobileIsTrue_ReturnsTrue()
        {
            var model = new ClientApplicationTypesModel{ NativeMobile = true, };

            model.IsComplete.Should().BeTrue();
        }

        [Test]
        public static void IsComplete_AllBooleanPropertiesFalse_ReturnsFalse()
        {
            var model = new ClientApplicationTypesModel();

            model.IsComplete.Should().BeFalse();
        }
    }
}
