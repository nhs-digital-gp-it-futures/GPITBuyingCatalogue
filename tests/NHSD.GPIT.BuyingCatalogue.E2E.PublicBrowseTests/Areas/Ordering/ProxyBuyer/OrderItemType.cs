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
    public sealed class OrderItemType
        : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderItemType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderTriageController),
                  nameof(OrderTriageController.OrderItemType),
                  Parameters,
                  UserSeedData.AliceEmail)
        {
        }

        [Fact]
        public void Submit_AssociatedService_NavigatesCorrectly()
        {
            CommonActions.ClickRadioButtonWithValue(CatalogueItemType.AssociatedService.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.SelectOrganisation)).Should().BeTrue();
        }

        [Fact]
        public void Submit_CatalogueSolution_NavigatesCorrectly()
        {
            CommonActions.ClickRadioButtonWithValue(CatalogueItemType.Solution.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.SelectOrganisation)).Should().BeTrue();
        }
    }
}
