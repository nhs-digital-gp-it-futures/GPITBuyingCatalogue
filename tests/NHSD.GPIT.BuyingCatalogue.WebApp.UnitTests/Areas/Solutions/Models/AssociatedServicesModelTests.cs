using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class AssociatedServicesModelTests
    {
        [Fact]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(AssociatedServicesModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Theory]
        [CommonAutoData]
        public static void HasServices_ValidServices_ReturnsTrue(AssociatedServicesModel model)
        {
            model.Services.Count.Should().BeGreaterThan(0);

            model.HasServices().Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void HasServices_NoService_ReturnsFalse(AssociatedServicesModel model)
        {
            model.Services = new List<AssociatedServiceModel>();

            model.HasServices().Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasServices_NullService_ReturnsFalse(AssociatedServicesModel model)
        {
            model.Services = null;

            model.HasServices().Should().BeFalse();
        }
    }
}
