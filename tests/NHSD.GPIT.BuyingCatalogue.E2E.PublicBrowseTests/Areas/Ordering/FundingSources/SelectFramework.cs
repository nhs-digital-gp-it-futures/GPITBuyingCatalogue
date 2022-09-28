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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.FundingSources
{
    public sealed class SelectFramework : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffIdMultipleFrameworks = new(90020, 1);
        private static readonly CallOffId CallOffIdSingleFramework = new(90021, 1);
        private static readonly CallOffId CallOffIdExistingFramework = new(90005, 1);

        private static readonly Dictionary<string, string> ParametersMF = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { "CallOffId", CallOffIdMultipleFrameworks.ToString() },
        };

        private static readonly Dictionary<string, string> ParametersSF = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { "CallOffId", CallOffIdSingleFramework.ToString() },
        };

        private static readonly Dictionary<string, string> ParametersEF = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { "CallOffId", CallOffIdExistingFramework.ToString() },
        };

        public SelectFramework(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(FundingSourceController),
                  nameof(FundingSourceController.SelectFramework),
                  ParametersMF,
                  testOutputHelper)
        {
        }

        [Fact]
        public async Task SelectFramework_SingleFramework_RedirectsAndSets()
        {
            await RunTestAsync(async () =>
            {
                NavigateToUrl(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.SelectFramework),
                    ParametersSF);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSources))
                .Should()
                .BeTrue();

                await using var dbcontext = GetEndToEndDbContext();

                var order = await dbcontext.Orders
                    .SingleAsync(o => o.Id == CallOffIdSingleFramework.Id
                                && o.OrderingParty.InternalIdentifier == InternalOrgId);

                var framework = await dbcontext.Orders
                    .Where(o => o.Id == CallOffIdSingleFramework.Id
                                && o.OrderingParty.InternalIdentifier == InternalOrgId)
                    .SelectMany(o => o.OrderItems
                                    .Where(oi =>
                                        oi.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.Solution)
                                    .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework)))
                    .ToListAsync();

                order.SelectedFrameworkId.Should().Be(framework.First().Id);

                Reset(CallOffIdSingleFramework);
            });
        }

        [Fact]
        public async Task SelectFramework_MultipleFrameworks_AllSectionsDisplayed()
        {
            await RunTestAsync(async () =>
            {
                await using var dbcontext = GetEndToEndDbContext();

                var frameworks = await dbcontext.Orders
                    .Where(o => o.Id == CallOffIdMultipleFrameworks.Id
                                && o.OrderingParty.InternalIdentifier == InternalOrgId)
                    .SelectMany(o => o.OrderItems
                                    .Where(oi =>
                                        oi.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.Solution)
                                    .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework)))
                    .ToListAsync();

                CommonActions.PageTitle().Should().BeEquivalentTo($"{SelectFrameworkModel.TitleText}-Order{CallOffIdMultipleFrameworks}".FormatForComparison());
                CommonActions.GoBackLinkDisplayed().Should().BeTrue();
                CommonActions.SaveButtonDisplayed().Should().BeTrue();
                CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(frameworks.Count);

                Reset(CallOffIdMultipleFrameworks);
            });
        }

        [Fact]
        public void SelectFramework_ClickGoback_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.Order))
                .Should()
                .BeTrue();
            });
        }

        [Fact]
        public void SelectFramework_ClickGobackWhileInError_ExpectedResult()
        {
            RunTest(() =>
             {
                 CommonActions.ClickSave();

                 CommonActions.ClickGoBackLink();

                 CommonActions.PageLoadedCorrectGetIndex(
                     typeof(OrderController),
                     nameof(OrderController.Order))
                 .Should()
                 .BeTrue();
             });
        }

        [Fact]
        public async Task SelectFramework_SelectFramework_SavesAndRedirects()
        {
            await RunTestAsync(async () =>
            {
                var optionId = CommonActions.GetRadioButtonsOptionsIds().First();

                CommonActions.ClickFirstRadio();

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSources))
                .Should()
                .BeTrue();

                using var dbcontext = GetEndToEndDbContext();

                var order = await dbcontext.Orders
                    .SingleAsync(o => o.Id == CallOffIdMultipleFrameworks.Id
                                && o.OrderingParty.InternalIdentifier == InternalOrgId);

                order.SelectedFrameworkId.Should().Be(optionId);

                Reset(CallOffIdMultipleFrameworks);
            });
        }

        [Fact]
        public void SelectFramework_HasError_DisplaysCorrectly()
        {
            RunTest(() =>
            {
                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.SelectFramework))
                .Should()
                .BeTrue();

                CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
                CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
                CommonActions.ElementIsDisplayed(Framework.Objects.Ordering.FundingSources.SelectFrameworkError).Should().BeTrue();
            });
        }

        [Fact]
        public void SelectFramework_ExistingFramework_RedirectsToConfirmPage()
        {
            RunTest(() =>
            {
                NavigateToUrl(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.SelectFramework),
                    ParametersEF);

                CommonActions.ClickFirstUnselectedRadio();

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.ConfirmFrameworkChange))
                .Should()
                .BeTrue();
            });
        }

        public void Dispose()
        {
            Reset(CallOffIdMultipleFrameworks);
            Reset(CallOffIdSingleFramework);
        }

        private async void Reset(CallOffId callOffId)
        {
            using var dbcontext = GetEndToEndDbContext();

            var order = await dbcontext.Orders
                .SingleAsync(o => o.Id == callOffId.Id
                            && o.OrderingParty.InternalIdentifier == InternalOrgId);

            order.SelectedFrameworkId = null;

            await dbcontext.SaveChangesAsync();
        }
    }
}
