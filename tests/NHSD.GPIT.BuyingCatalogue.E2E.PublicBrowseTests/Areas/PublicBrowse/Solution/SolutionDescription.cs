﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class SolutionDescription : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public SolutionDescription(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.Description),
                  Parameters)
        {
        }

        [Fact]
        public async Task Description_InRemediationDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);
            solution.PublishedStatus = PublicationStatus.InRemediation;
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions
                .ElementExists(Objects.PublicBrowse.SolutionObjects.InRemediationNotice)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Description_InRemediationNotDisplayed()
        {
            CommonActions
                .ElementExists(Objects.PublicBrowse.SolutionObjects.InRemediationNotice)
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task Description_IsSuspended_SuspendedNoticeDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);
            solution.PublishedStatus = PublicationStatus.Suspended;
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions
                .ElementExists(Objects.PublicBrowse.SolutionObjects.SolutionSuspendedNotice)
                .Should()
                .BeTrue();

            CommonActions
                .ElementExists(Objects.PublicBrowse.SolutionObjects.SolutionNavigationMenu)
                .Should()
                .BeFalse();

            CommonActions
                .ElementExists(CommonSelectors.PaginationNext)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Description_NotSuspended_SuspendedNoticeNotDisplayed()
        {
            CommonActions
                .ElementExists(Objects.PublicBrowse.SolutionObjects.SolutionSuspendedNotice)
                .Should()
                .BeFalse();

            CommonActions
                .ElementExists(Objects.PublicBrowse.SolutionObjects.SolutionNavigationMenu)
                .Should()
                .BeTrue();

            CommonActions
                .ElementExists(CommonSelectors.PaginationNext)
                .Should()
                .BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.Single(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
