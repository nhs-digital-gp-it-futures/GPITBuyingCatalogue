using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.FundingSources
{
    public sealed class ConfirmChangeFramework : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const string SelectedFrameworkId = "DFOCVC001";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { "CallOffId", CallOffId.ToString() },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { nameof(SelectedFrameworkId), SelectedFrameworkId },
        };

        public ConfirmChangeFramework(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(FundingSourceController),
                  nameof(FundingSourceController.ConfirmFrameworkChange),
                  Parameters,
                  testOutputHelper,
                  QueryParameters)
        {
        }

        [Fact]
        public void ConfirmFrameworkChange_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.PageTitle().Should().BeEquivalentTo($"{ConfirmFrameworkChangeModel.TitleText}-Order{CallOffId}".FormatForComparison());
                CommonActions.GoBackLinkDisplayed().Should().BeTrue();
                CommonActions.SaveButtonDisplayed().Should().BeTrue();
                CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(2);
            });
        }

        [Fact]
        public void ConfirmFrameworkChange_ClickGoback_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.SelectFramework))
                .Should()
                .BeTrue();
            });
        }

        [Fact]
        public void ConfirmFrameworkChange_ClickGobackWhileInError_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickSave();

                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.SelectFramework))
                .Should()
                .BeTrue();
            });
        }

        [Fact]
        public void ConfirmFrameworkChange_HasError_DisplaysCorrectly()
        {
            RunTest(() =>
            {
                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.ConfirmFrameworkChange))
                .Should()
                .BeTrue();

                CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
                CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
                CommonActions.ElementIsDisplayed(Framework.Objects.Ordering.FundingSources.ConfirmChangeError)
                .Should().BeTrue();
            });
        }

        [Fact]
        public async Task ConfirmFrameworkChange_SelectYes_SavesAndRedirects()
        {
            await RunTestAsync(async () =>
            {
                CommonActions.ClickFirstRadio();

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSources))
                .Should()
                .BeTrue();

                using var dbcontext = GetEndToEndDbContext();

                var order = await dbcontext.Orders
                    .SingleAsync(o => o.Id == CallOffId.OrderNumber
                                && o.OrderingParty.InternalIdentifier == InternalOrgId);

                order.SelectedFrameworkId.Should().Be(SelectedFrameworkId);

                Reset();
            });
        }

        [Fact]
        public void ConfirmFrameworkChange_SelectNo_Redirects()
        {
            RunTest(() =>
            {
                CommonActions.ClickLastRadio();

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.SelectFramework))
                .Should()
                .BeTrue();
            });
        }

        public void Dispose()
        {
            Reset();
        }

        private async void Reset()
        {
            using var dbcontext = GetEndToEndDbContext();

            var order = await dbcontext.Orders
                .SingleAsync(o => o.Id == CallOffId.OrderNumber
                            && o.OrderingParty.InternalIdentifier == InternalOrgId);

            order.SelectedFrameworkId = "DFOCVC001";

            await dbcontext.SaveChangesAsync();
        }
    }
}
