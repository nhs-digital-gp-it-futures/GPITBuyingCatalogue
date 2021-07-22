using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    internal static class OrderSeedData
    {
        internal static void Initialize(BuyingCatalogueDbContext context)
        {
            AddOrderAtDescriptionStage(context);
            AddOrderAtCallOffPartyStage(context);
            AddOrderAtSupplierStage(context);
            AddOrderAtCommencementDateStage(context);
            context.SaveChanges();
        }

        private static void AddOrderAtDescriptionStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90000;
            var timeNow = DateTime.UtcNow;

            var organisationId = context.Organisations.Where(o => o.OdsCode == "03F").FirstOrDefault().OrganisationId;

            var order = new Order
            {
                Id = orderId,
                Revision = 1,
                OrderingPartyId = organisationId,
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
            };

            var user = context.Users.Where(u => u.OrganisationFunction == "Buyer" && u.PrimaryOrganisationId == organisationId).FirstOrDefault();

            order.SetLastUpdatedBy(
                new Guid(user.Id),
                $"{user.FirstName} {user.LastName}");

            context.Add(order);
        }

        private static void AddOrderAtCallOffPartyStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90001;
            var timeNow = DateTime.UtcNow;

            var organisationId = context.Organisations.Where(o => o.OdsCode == "03F").FirstOrDefault().OrganisationId;

            var order = new Order
            {
                Id = orderId,
                Revision = 1,
                OrderingPartyId = organisationId,
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
            };

            var user = context.Users.Where(u => u.OrganisationFunction == "Buyer" && u.PrimaryOrganisationId == organisationId).FirstOrDefault();

            order.SetLastUpdatedBy(
                new Guid(user.Id),
                $"{user.FirstName} {user.LastName}");

            context.Add(order);
        }

        private static void AddOrderAtSupplierStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90002;
            var timeNow = DateTime.UtcNow;

            var organisationId = context.Organisations.Where(o => o.OdsCode == "03F").FirstOrDefault().OrganisationId;

            var order = new Order
            {
                Id = orderId,
                Revision = 1,
                OrderingPartyId = organisationId,
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new()
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.com",
                    Phone = "123456789",
                },
            };

            var user = context.Users.Where(u => u.OrganisationFunction == "Buyer" && u.PrimaryOrganisationId == organisationId).FirstOrDefault();

            order.SetLastUpdatedBy(
                new Guid(user.Id),
                $"{user.FirstName} {user.LastName}");

            context.Add(order);
        }

        private static void AddOrderAtCommencementDateStage(BuyingCatalogueDbContext context)
        {
            var orderId = 90003;
            var timenow = DateTime.UtcNow;

            var organisationId = context.Organisations.Where(o => o.OdsCode == "03F").FirstOrDefault().OrganisationId;

            var order = new Order
            {
                Id = orderId,
                Revision = 1,
                OrderingPartyId = organisationId,
                Created = timenow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new()
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = "99997",
                SupplierContact = new()
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
            };

            var user = context.Users.Where(u => u.OrganisationFunction == "Buyer" && u.PrimaryOrganisationId == organisationId).FirstOrDefault();

            order.SetLastUpdatedBy(
                new Guid(user.Id),
                $"{user.FirstName} {user.LastName}");

            context.Add(order);
        }
    }
}
