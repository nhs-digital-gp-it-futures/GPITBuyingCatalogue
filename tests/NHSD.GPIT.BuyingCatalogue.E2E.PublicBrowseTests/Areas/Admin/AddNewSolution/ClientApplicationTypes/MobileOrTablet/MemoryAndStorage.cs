﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.MobileOrTablet
{
    public sealed class MemoryAndStorage : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public MemoryAndStorage(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(MobileTabletBasedController),
                  nameof(MobileTabletBasedController.MemoryAndStorage),
                  Parameters)
        {
        }

        [Fact]
        public async Task MemoryAndStorage_SaveData()
        {
            var memorySize = CommonActions.SelectRandomDropDownItem(Objects.Admin.EditSolution.ClientApplicationObjects.MinimumMemoryDropDown);

            var description = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.Description, 300);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var solution = await context.Solutions.SingleOrDefaultAsync(s => s.CatalogueItemId == SolutionId);

            var memoryAndStorage = JsonSerializer.Deserialize<ServiceContracts.Solutions.ClientApplication>(
                solution.ClientApplication, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?.MobileMemoryAndStorage;

            memoryAndStorage.Should().NotBeNull();
            memoryAndStorage.MinimumMemoryRequirement.Should().Be(memorySize);
            memoryAndStorage.Description.Should().Be(description);
        }

        [Fact]
        public void MemoryAndStorage_ErrorThrownMissingMandatory()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public void MemoryAndStorage_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
