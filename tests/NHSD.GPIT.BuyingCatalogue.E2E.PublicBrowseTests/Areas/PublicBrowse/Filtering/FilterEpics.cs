﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Filtering
{
    [Collection(nameof(SharedContextCollection))]
    public class FilterEpics : BuyerTestBase
    {
        private static readonly IEnumerable<int> CapabilityIds = new[] { 1, 2, 3, 4, 5 };
        private static readonly Dictionary<int, string[]> CapabilitiesAndEpics = new()
        {
            { 1, Array.Empty<string>() },
            { 2, Array.Empty<string>() },
            { 3, Array.Empty<string>() },
            { 4, Array.Empty<string>() },
            { 5, Array.Empty<string>() },
        };

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { "selected", CapabilitiesAndEpics.ToFilterString() },
        };

        public FilterEpics(LocalWebApplicationFactory factory)
            : base(factory, typeof(FilterController), nameof(FilterController.FilterEpics), null, null, Parameters)
        {
        }

        [Fact]
        public void FilterEpics_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(FilterObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.CatalogueSolutionsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.CapabilitiesBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.EditCapabilitiesLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var expected = context.Epics.Count(
                x => x.Capabilities.Any(y => CapabilitiesAndEpics.Keys.Contains(y.Id)) && x.IsActive
                    && context.CatalogueItemEpics.Any(y => x.Id == y.EpicId));

            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(expected);
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);
        }

        [Fact]
        public void FilterEpics_WithSelectedIds_AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var selected = new Dictionary<int, string[]>(CapabilitiesAndEpics.Select(c => new KeyValuePair<int, string[]>(
                c.Key,
                context.Epics
                    .Where(x => x.Capabilities.Any(y => y.Id == c.Key
                        && x.IsActive
                        && context.CatalogueItemEpics.Any(y => x.Id == y.EpicId)))
                    .Select(x => x.Id)
                    .ToArray())));

            NavigateToUrl(
                typeof(FilterController),
                nameof(FilterController.FilterEpics),
                null,
                new Dictionary<string, string>
                {
                    { "selected", selected.ToFilterString() },
                });

            CommonActions.ElementIsDisplayed(FilterObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.CatalogueSolutionsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.CapabilitiesBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.EditCapabilitiesLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(selected.Sum(kv => kv.Value.Length));
            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(selected.Sum(kv => kv.Value.Length));
        }

        [Fact]
        public void FilterEpics_ClickHomeBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void FilterEpics_ClickCatalogueSolutionsBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.CatalogueSolutionsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void FilterEpics_ClickCapabilitiesBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.CapabilitiesBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.FilterCapabilities)).Should().BeTrue();
        }

        [Fact]
        public void FilterEpics_ClickEditCapabilitiesLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.EditCapabilitiesLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.FilterCapabilities)).Should().BeTrue();
        }

        [Fact]
        public void FilterEpics_ClickSave_NoSelectionMade_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.FilterEpics)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
        }

        [Fact]
        public void FilterEpics_ClickSave_SelectionMade_ExpectedResult()
        {
            CommonActions.ClickAllCheckboxes();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index)).Should().BeTrue();
        }
    }
}
