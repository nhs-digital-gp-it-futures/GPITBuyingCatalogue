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
    public sealed class Connectivity : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public Connectivity(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(MobileTabletBasedController),
                  nameof(MobileTabletBasedController.Connectivity),
                  Parameters)
        {
        }

        [Fact]
        public async Task Connectivity_SavePage()
        {
            var connectionSpeed = CommonActions.SelectRandomDropDownItem(Objects.Admin.AddSolutionObjects.ConnectivityDropdown);

            int numCheckBoxes = CommonActions.GetNumberOfCheckBoxesDisplayed();

            var supportedConnectionType = CommonActions.ClickCheckbox(Objects.Common.CommonSelectors.CheckboxItem, new Random().Next(numCheckBoxes));

            var description = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.Description, 300);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == SolutionId);

            var mobileConnectionDetails = JsonSerializer.Deserialize<ServiceContracts.Solutions.ClientApplication>(solution.ClientApplication)?.MobileConnectionDetails;

            mobileConnectionDetails.Should().NotBeNull();

            mobileConnectionDetails.ConnectionType.Should().ContainEquivalentOf(supportedConnectionType);
            mobileConnectionDetails.MinimumConnectionSpeed.Should().Be(connectionSpeed);
            mobileConnectionDetails.Description.Should().Be(description);
        }

        [Fact]
        public void Connectivity_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet)).Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
