﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class MemoryAndStorage : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public MemoryAndStorage(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-002/section/native-mobile/memory-and-storage")
        {
            AuthorityLogin();
        }

        [Fact]
        public async Task MemoryAndStorage_CompleteAllFields()
        {
            CommonActions.SelectDropDownItem(CommonSelectors.MemorySelect, 1);

            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 200);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "002"))).ClientApplication;

            clientApplication.Should().ContainEquivalentOf("MinimumMemoryRequirement\":\"256MB\"");
            clientApplication.Should().ContainEquivalentOf($"Description\":\"{description}\"");
        }

        [Fact]
        public void MemoryAndStorage_SectionComplete()
        {
            CommonActions.SelectDropDownItem(CommonSelectors.MemorySelect, 1);

            TextGenerators.TextInputAddText(CommonSelectors.Description, 200);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Memory and storage").Should().BeTrue();
        }

        [Fact]
        public void MemoryAndStorage_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Memory and storage").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "002"));
        }
    }
}
