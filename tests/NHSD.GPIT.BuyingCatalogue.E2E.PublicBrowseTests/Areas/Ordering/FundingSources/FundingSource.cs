using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource;
using Xunit;
using Xunit.Abstractions;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.FundingSources
{
    [Collection(nameof(OrderingCollection))]
    public sealed class FundingSource : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90006, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { "CallOffId", CallOffId.ToString() },
            { nameof(CatalogueItemId), CatalogueItemId.ToString() },
        };

        public FundingSource(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                    factory,
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSource),
                    Parameters,
                    testOutputHelper)
        {
        }

        [Fact]
        public async Task FundingSource_AllSectionsDisplayed()
        {
            await RunTestAsync(async () =>
            {
                await using var dbcontext = GetEndToEndDbContext();

                var catalogueItem = dbcontext.CatalogueItems.First(ci => ci.Id == CatalogueItemId);

                CommonActions.PageTitle().Should().BeEquivalentTo($"Funding source - {catalogueItem.Name}".FormatForComparison());
                CommonActions.GoBackLinkDisplayed().Should().BeTrue();
                CommonActions.SaveButtonDisplayed().Should().BeTrue();
                CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(3);
            });
        }

        [Fact]
        public void FundingSource_NoInput_Errors()
        {
            RunTest(() =>
            {
                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSource))
                    .Should().BeTrue();

                CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
                CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

                CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.FundingSourceError).Should().BeTrue();
            });
        }

        [Fact]
        public async Task FundingSource_ExpectedInput_CorrectResults()
        {
            await RunTestAsync(async () =>
            {
                CommonActions.ClickRadioButtonWithText("Mixed");

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSources))
                    .Should().BeTrue();

                await using var dbContext = GetEndToEndDbContext();

                var orderId = await dbContext.OrderId(CallOffId);

                var orderItemSaved = await dbContext.OrderItems
                    .Include(oi => oi.OrderItemFunding)
                    .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.CatalogueItemId == CatalogueItemId);

                orderItemSaved?.FundingType.Should().Be(OrderItemFundingType.MixedFunding);
            });
        }

        public void Dispose()
        {
            using var dbContext = GetEndToEndDbContext();

            var orderId = dbContext.OrderId(CallOffId).Result;

            var orderItem = dbContext.OrderItems
                .Include(oi => oi.OrderItemFunding)
                .First(oi => oi.OrderId == orderId && oi.CatalogueItemId == CatalogueItemId);

            orderItem.OrderItemFunding = null;

            dbContext.SaveChanges();
        }
    }
}
