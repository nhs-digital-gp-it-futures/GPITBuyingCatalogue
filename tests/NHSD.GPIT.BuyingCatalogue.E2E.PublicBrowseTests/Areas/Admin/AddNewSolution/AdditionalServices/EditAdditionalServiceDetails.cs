﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AdditionalServices
{
    public sealed class EditAdditionalServiceDetails : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly CatalogueItemId AdditionalServiceId = new(99999, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public EditAdditionalServiceDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.EditAdditionalServiceDetails),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditAdditionalServiceDetails_UpdateValidValues()
        {
            CommonActions.ClearInputElement(CommonSelectors.Name);
            CommonActions.ClearInputElement(CommonSelectors.Description);

            var name = TextGenerators.TextInputAddText(CommonSelectors.Name, 255);
            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var additionalService = await context.CatalogueItems
                .Include(ci => ci.AdditionalService)
                .SingleAsync(ci => ci.Id == AdditionalServiceId);

            additionalService.Name.Should().Be(name);
            additionalService.AdditionalService.FullDescription.Should().Be(description);
        }

        [Fact]//////////////////////////////////////////////////////////////////////////////////////////////////
        public void EditAdditionalServiceDetails_MissingDataThrowsError()
        {
            CommonActions.ClearInputElement(CommonSelectors.Name);
            CommonActions.ClearInputElement(CommonSelectors.Description);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalServiceDetails))
                .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AdditionalServicesObjects.AdditionalServiceNameError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AdditionalServicesObjects.AdditionalServiceDescriptionError).Should().BeTrue();
        }

    }
}
