using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.FundingSource
{
    public static class FundingSourceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void AvailableFundingSources_Expected(
            FundingSourceModel model)
        {
            var selectList = new List<SelectListItem>
            {
                new("Central funding", ServiceContracts.Enums.FundingSource.Central.ToString()),
                new("Local funding", ServiceContracts.Enums.FundingSource.Local.ToString()),
            };

            model.AvailableFundingSources.Should().BeEquivalentTo(selectList);
        }
    }
}
