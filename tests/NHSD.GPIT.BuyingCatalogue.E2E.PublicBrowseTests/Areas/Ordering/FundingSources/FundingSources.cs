using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.FundingSources
{
    [Collection(nameof(OrderingCollection))]
    public sealed class FundingSources : BuyerTestBase
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId DFOCVCCallOffId = new(90005, 1);
        private static readonly CallOffId GPITFuturesCallOffId = new(90006, 1);
        private static readonly CallOffId NoFundingCallOffId = new(90015, 1);

        public FundingSources(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(FundingSourceController),
                 nameof(FundingSourceController.FundingSources),
                 GetParameters(GPITFuturesCallOffId))
        {
        }

        [Theory]
        [InlineData(90033, 1, true)]
        [InlineData(90033, 2, false)]
        public void FundingSources_MultipleFrameworks_AllSectionsDisplayed(int orderNumber, int revision, bool changeFrameworkDisplayed)
        {
            var callOffId = new CallOffId(orderNumber, revision);

            NavigateToUrl(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources),
                GetParameters(callOffId));

            CommonActions.PageTitle().Should().BeEquivalentTo($"Funding sources - Order {callOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.ChangeFramework).Should().Be(changeFrameworkDisplayed);
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.LocalOnlyFundingSourcesTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.EditableFundingSourcesTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.NoFundingRequiredSourcesTable).Should().BeFalse();
        }

        [Fact]
        public void FundingSources_DFOCVCSolution_AllSectionsDisplayed()
        {
            NavigateToUrl(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources),
                GetParameters(DFOCVCCallOffId));

            CommonActions.PageTitle().Should().BeEquivalentTo($"Funding sources - Order {DFOCVCCallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.ChangeFramework).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.LocalOnlyFundingSourcesTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.EditableFundingSourcesTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.NoFundingRequiredSourcesTable).Should().BeFalse();
        }

        [Fact]
        public void FundingSources_GPITFuturesSolution_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Funding sources - Order {GPITFuturesCallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.ChangeFramework).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.LocalOnlyFundingSourcesTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.EditableFundingSourcesTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.NoFundingRequiredSourcesTable).Should().BeFalse();
        }

        [Fact]
        public void FundingSources_NoFundingReqired_AllSectionsDisplayed()
        {
            NavigateToUrl(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources),
                GetParameters(NoFundingCallOffId));

            CommonActions.PageTitle().Should().BeEquivalentTo($"Funding sources - Order {NoFundingCallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.ChangeFramework).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.LocalOnlyFundingSourcesTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.EditableFundingSourcesTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.FundingSources.NoFundingRequiredSourcesTable).Should().BeTrue();
        }

        [Fact]
        public void FundingSources_ClickGoBack_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
                .Should().BeTrue();
        }

        [Fact]
        public void FundingSources_ClickContinue_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
                .Should().BeTrue();
        }

        [Fact]
        public void FundingSources_ClickEdit_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.FundingSources.EditLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSource))
                .Should().BeTrue();
        }

        private static Dictionary<string, string> GetParameters(CallOffId callOffId)
        {
            return new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { "CallOffId", callOffId.ToString() },
            };
        }
    }
}
