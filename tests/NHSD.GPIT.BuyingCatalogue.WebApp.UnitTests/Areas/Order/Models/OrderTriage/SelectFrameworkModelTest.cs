using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.OrderTriage
{
    public static class SelectFrameworkModelTest
    {
        [Theory]
        [MockAutoData]
        public static void PropertiesCorrectlySet(
            Organisation organisation,
            IList<EntityFramework.Catalogue.Models.Framework> frameworks,
            string selectedFrameworkId)
        {
            var expectedFrameworks = frameworks.Select(
                    f => new SelectOption<string>(
                        $"{f.ShortName}",
                        f.Id))
                .ToList();

            var model = new SelectFrameworkModel(organisation, frameworks, selectedFrameworkId);

            model.OrganisationName.Should().Be(organisation.Name);
            model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
            model.SelectedFrameworkId.Should().Be(selectedFrameworkId);
            model.Frameworks.Should().BeEquivalentTo(expectedFrameworks);
        }
    }
}
