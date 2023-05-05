using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.Filters
{
    public static class AdditionalFilterModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_WithFrameworks_CreatesFrameworkOptions(List<FrameworkFilterInfo> frameworks)
        {
            var model = new AdditionalFiltersModel(frameworks, null);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);

            foreach (var framework in frameworks)
            {
                model.FrameworkOptions.Should().ContainEquivalentOf(new SelectOption<string>
                {
                    Value = framework.Id,
                    Text = $"{framework.ShortName} ({framework.CountOfActiveSolutions})",
                    Selected = false,
                });
            }
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithClientApplicationTypeSelected_CreatesClientApplicationTypeCheckBoxItems(string clientApplicationTypeSelected)
        {
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), clientApplicationTypeSelected);

            model.ClientApplicationTypeCheckBoxItems.Should().NotBeNull();
            model.ClientApplicationTypeCheckBoxItems.Should().HaveCount(3);

            foreach (var item in model.ClientApplicationTypeCheckBoxItems)
            {
                if (clientApplicationTypeSelected != null && clientApplicationTypeSelected.Contains(item.ClientApplicationType.EnumMemberName()))
                    item.IsSelected.Should().BeTrue();
                else
                    item.IsSelected.Should().BeFalse();
            }
        }
    }
}
