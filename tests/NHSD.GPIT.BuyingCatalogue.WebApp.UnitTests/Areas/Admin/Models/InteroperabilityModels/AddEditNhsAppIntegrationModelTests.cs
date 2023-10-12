using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.InteroperabilityModels
{
    public static class AddEditNhsAppIntegrationModelTests
    {
        [Fact]
        public static void CheckIntegrationTypes_WhenIntegrationsIsNull_ShouldNotThrowException()
        {
            var model = new AddEditNhsAppIntegrationModel();

            Action action = () => model.CheckIntegrationTypes(null);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public static void CheckIntegrationTypes_WhenNhsAppIntegrationNotFound_ShouldNotChangeIntegrationIdAndCheckedProperties()
        {
            var model = new AddEditNhsAppIntegrationModel();
            var integrations = new List<Integration>();

            model.CheckIntegrationTypes(integrations);

            model.IntegrationId.Should().Be(Guid.Empty);
            model.NhsAppIntegrationTypes.Should().BeNull();
        }
    }
}
