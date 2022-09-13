﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Filtering
{
    public class FilterCapabilities : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public FilterCapabilities(LocalWebApplicationFactory factory)
            : base(factory, typeof(FilterController), nameof(FilterController.FilterCapabilities), null)
        {
        }

        [Fact]
        public void FilterCapabilities_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(FilterObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.CatalogueSolutionsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            var expected = GetEndToEndDbContext().Capabilities.Count();

            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(expected);
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);
        }

        [Fact]
        public void FilterCapabilities_WithSelectedIds_AllSectionsDisplayed()
        {
            var selectedIds = GetEndToEndDbContext().Capabilities.Select(x => x.Id).ToList();

            NavigateToUrl(
                typeof(FilterController),
                nameof(FilterController.FilterCapabilities),
                null,
                new Dictionary<string, string>
                {
                    { "selectedCapabilityIds", string.Join(FilterConstants.Delimiter, selectedIds) },
                });

            CommonActions.ElementIsDisplayed(FilterObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.CatalogueSolutionsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(selectedIds.Count);
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(selectedIds.Count);
        }

        [Fact]
        public void FilterCapabilities_ClickHomeBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void FilterCapabilities_ClickCatalogueSolutionsBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.CatalogueSolutionsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void FilterCapabilities_ClickSave_NoSelectionMade_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.FilterCapabilities)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
        }

        [Fact]
        public void FilterCapabilities_ClickSave_SelectionMade_ExpectedResult()
        {
            CommonActions.ClickAllCheckboxes();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.IncludeEpics)).Should().BeTrue();
        }
    }
}
