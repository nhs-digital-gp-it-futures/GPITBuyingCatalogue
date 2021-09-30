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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.Desktop
{
    public sealed class HardwareRequirements : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public HardwareRequirements(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DesktopBasedController),
                  nameof(DesktopBasedController.HardwareRequirements),
                  Parameters)
        {
        }

        [Fact]
        public async Task HardwareRequirements_SavePage()
        {
            var expectedRequirements = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.Description, 500);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == SolutionId);

            var hardwareRequirements = JsonSerializer.Deserialize<ServiceContracts.Solutions.ClientApplication>(
                solution.ClientApplication, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?.NativeDesktopHardwareRequirements;

            hardwareRequirements.Should().NotBeNull();
            hardwareRequirements.Should().Be(expectedRequirements);
        }

        [Fact]
        public void HardwareRequirements_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
