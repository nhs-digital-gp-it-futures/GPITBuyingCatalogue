using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    internal static class OrderSeedData
    {
        internal static void Initialize(BuyingCatalogueDbContext context)
        {
            AddOrderAtDescriptionStage(context);
            context.SaveChanges();
        }

        private static void AddOrderAtDescriptionStage(BuyingCatalogueDbContext context)
        {
            var orderId = 90000;
            var timenow = DateTime.UtcNow;

            var organisationId = context.Organisations.Where(o => o.OdsCode == "03F").FirstOrDefault().OrganisationId;

            var order = new Order
            {
                Id = orderId,
                Revision = 1,
                CallOffId = new CallOffId(orderId, 1),
                OrderingPartyId = organisationId,
                Created = timenow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
            };

            var user = context.Users.Where(u => u.OrganisationFunction == "Buyer" && u.PrimaryOrganisationId == organisationId).FirstOrDefault();

            order.SetLastUpdatedBy(
                new Guid(user.Id),
                $"{user.FirstName} {user.LastName}");

            context.Add(order);
        }
    }
}
