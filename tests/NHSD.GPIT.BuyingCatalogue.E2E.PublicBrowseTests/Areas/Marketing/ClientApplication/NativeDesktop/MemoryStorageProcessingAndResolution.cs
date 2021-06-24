﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    public sealed class MemoryStorageProcessingAndResolution : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public MemoryStorageProcessingAndResolution(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop/memory-and-storage")
        {
        }

        [Fact]
        public async Task MemoryStorageProcessingAndResolution_CompleteAllFields()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.MemorySelect, 1);

            var storageSpace = TextGenerators.TextInputAddText(CommonSelectors.StorageDescriptionTextArea, 100);

            var processingPower = TextGenerators.TextInputAddText(CommonSelectors.MinimumCpuTextArea, 100);

            CommonActions.SelectDropdownItem(CommonSelectors.ResolutionSelect, 1);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@"MinimumMemoryRequirement"":""256MB""");
            clientApplication.Should().ContainEquivalentOf($"StorageRequirementsDescription\":\"{storageSpace}\"");
            clientApplication.Should().ContainEquivalentOf($"MinimumCpu\":\"{processingPower}\"");
            clientApplication.Should().ContainEquivalentOf(@"RecommendedResolution"":""16:9 - 640 x 360");
        }

        [Fact]
        public void MemoryStorageProcessingAndResolution_SectionComplete()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.MemorySelect, 1);

            TextGenerators.TextInputAddText(CommonSelectors.StorageDescriptionTextArea, 300);

            TextGenerators.TextInputAddText(CommonSelectors.MinimumCpuTextArea, 300);

            CommonActions.SelectDropdownItem(CommonSelectors.ResolutionSelect, 1);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Memory, storage, processing and resolution").Should().BeTrue();
        }

        [Fact]
        public void MemoryStorageProcessingAndResolution_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Memory, storage, processing and resolution").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
