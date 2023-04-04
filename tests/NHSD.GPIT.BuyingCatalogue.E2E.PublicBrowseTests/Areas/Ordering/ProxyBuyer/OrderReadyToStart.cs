using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.ProxyBuyer
{
    [Collection(nameof(OrderingCollection))]
    public sealed class OrderReadyToStart
        : BuyerTestBase
    {
        private const string InternalOrgId = "CG-15F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderReadyToStart(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderController),
                  nameof(OrderController.ReadyToStart),
                  Parameters,
                  UserSeedData.AliceEmail)
        {
        }

        [Fact]
        public void AssociatedService_ClickBackLink_RedirectsToCorrectPage()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "orderType", CatalogueItemType.AssociatedService.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart),
                Parameters,
                queryParams);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.SelectOrganisation)).Should().BeTrue();
        }
    }
}
