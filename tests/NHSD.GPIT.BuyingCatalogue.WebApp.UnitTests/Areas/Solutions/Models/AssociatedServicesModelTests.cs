using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AssociatedServicesModelTests
    {
        [Test]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(AssociatedServicesModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Test, AutoData]
        public static void HasServices_ValidServices_ReturnsTrue(AssociatedServicesModel model)
        {
            model.Services.Count.Should().BeGreaterThan(0);

            model.HasServices().Should().BeTrue();
        }

        [Test, AutoData]
        public static void HasServices_NoService_ReturnsFalse(AssociatedServicesModel model)
        {
            model.Services = new List<AssociatedServiceModel>();

            model.HasServices().Should().BeFalse();
        }

        [Test, AutoData]
        public static void HasServices_NullService_ReturnsFalse(AssociatedServicesModel model)
        {
            model.Services = null;

            model.HasServices().Should().BeFalse();
        }
    }
}
