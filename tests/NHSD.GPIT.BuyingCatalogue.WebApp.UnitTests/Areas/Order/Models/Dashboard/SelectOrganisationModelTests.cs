using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard;
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
            var currentOdsCode = organisations.First().OdsCode;

            var model = new SelectOrganisationModel(currentOdsCode, organisations);

            model.BackLink.Should().Be($"/order/organisation/{currentOdsCode}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be("Which organisation are you looking for?");
            model.AvailableOrganisations.Should().BeEquivalentTo(organisations);
            model.SelectedOrganisation.Should().Be(currentOdsCode);
        }
    }
}
