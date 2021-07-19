using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class SupplierInformation
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CallOffId CallOffId = new(90002, 1);

        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" }, { "CallOffId", CallOffId.ToString() } };

        public SupplierInformation(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SupplierController),
                  nameof(SupplierController.Supplier),
                  Parameters)
        {
        }

        [Fact]
        public void SupplierInformation_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Find supplier information for {CallOffId}");
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierSearchInput).Should().BeTrue();
        }



        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Single(o => o.Id == CallOffId.Id);

            order.Supplier = null;
            order.SupplierContact = null;

            context.SaveChanges();
        }
    }
}
