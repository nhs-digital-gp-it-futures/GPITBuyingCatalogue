using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.OrderTriage
{
    public static class SelectFrameworkModelTest
    {
        [Theory]
        [MockAutoData]
        public static void PropertiesCorrectlySet(
            string organisationName, 
            IList<EntityFramework.Catalogue.Models.Framework> frameworks, 
            string selectedFrameworkId
            )
        {
            var expectedFrameworks = frameworks.Select(
                    f => new SelectOption<string>(
                        $"{f.ShortName}{(f.IsExpired ? " (Expired)" : string.Empty)}",
                        f.Id,
                        disabled: f.IsExpired))
                .ToList();

            var model = new SelectFrameworkModel(organisationName, frameworks, selectedFrameworkId);

            model.OrganisationName.Should().Be(organisationName);
            model.SelectedFrameworkId.Should().Be(selectedFrameworkId);
            model.Frameworks.Should().BeEquivalentTo(expectedFrameworks);
        }
    }
}
