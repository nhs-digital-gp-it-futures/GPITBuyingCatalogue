﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ApplicationTypes.Desktop
{
    [Collection(nameof(AdminCollection))]
    public sealed class AdditionalInformation : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AdditionalInformation(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DesktopBasedController),
                  nameof(DesktopBasedController.AdditionalInformation),
                  Parameters)
        {
        }

        [Fact]
        public async Task AdditionalInformation_SavePage()
        {
            var expectedInformation = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.AdditionalInfoTextArea, 500);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);

            var additionalInformation = JsonDeserializer.Deserialize<ServiceContracts.Solutions.ApplicationTypes>(solution.ApplicationType)
                ?.NativeDesktopAdditionalInformation;

            additionalInformation.Should().NotBeNull();
            additionalInformation.Should().Be(expectedInformation);
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
