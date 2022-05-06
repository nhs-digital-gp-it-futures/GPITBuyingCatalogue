using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Dashboard
{
    public static class SelectOrganisationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            List<Organisation> organisations)
        {
            var currentInternalOrgId = organisations.First().InternalIdentifier;

            var model = new SelectOrganisationModel(currentInternalOrgId, organisations);

            model.Title.Should().Be("Which organisation are you looking for?");
            model.AvailableOrganisations.Should().BeEquivalentTo(organisations);
            model.SelectedOrganisation.Should().Be(currentInternalOrgId);
        }
    }
}
