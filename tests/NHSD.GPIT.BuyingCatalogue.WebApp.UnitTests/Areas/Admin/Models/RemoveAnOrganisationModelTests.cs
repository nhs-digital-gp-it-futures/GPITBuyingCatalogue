using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class RemoveAnOrganisationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Organisation currentOrganisation,
            Organisation relatedOrganisation)
        {
            var model = new RemoveAnOrganisationModel(currentOrganisation, relatedOrganisation);

            model.CurrentOrganisation.Should().Be(currentOrganisation);
            model.RelatedOrganisation.Should().Be(relatedOrganisation);
        }
    }
}
