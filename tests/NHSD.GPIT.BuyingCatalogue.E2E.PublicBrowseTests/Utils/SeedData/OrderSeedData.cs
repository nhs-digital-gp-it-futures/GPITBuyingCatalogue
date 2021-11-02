using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

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
            AddOrderAtCatalogueSolutionStage(context);
            AddOrderWithAddedCatalogueSolution(context);
            AddOrderWithAddedNoContactCatalogueSolution(context);
            AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolution(context);
            AddOrderWithAddedAssociatedService(context);
            AddOrderReadyToComplete(context);
            AddCompletedOrder(context);
            context.SaveChanges();
        }

        private static void AddOrderAtDescriptionStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90000;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            context.Add(order);
        }

        private static void AddOrderAtCallOffPartyStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90001;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            context.Add(order);
        }

        private static void AddOrderAtSupplierStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90002;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.com",
                    Phone = "123456789",
                },
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxx CONSIDER IF THIS IS REQUIRED. WOULD SEEDING HAVE User.Id in context? Is setting this here necessary??
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            context.Add(order);
        }

        private static void AddOrderAtCommencementDateStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90003;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = 99997,
                SupplierContact = new Contact
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            context.Add(order);
        }

        private static void AddOrderAtCatalogueSolutionStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90004;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = 99998,
                SupplierContact = new Contact
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
                CommencementDate = timeNow.AddDays(1),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            context.Add(order);
        }

        private static void AddOrderWithAddedCatalogueSolution(BuyingCatalogueDbContext context)
        {
            const int orderId = 90005;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = 99999,
                SupplierContact = new Contact
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
                CommencementDate = DateTime.UtcNow.AddDays(1),
            };

            var price = context.CatalogueItems
                .Include(c => c.CataloguePrices).ThenInclude(s => s.PricingUnit)
                .Single(c => c.Id == new CatalogueItemId(99999, "001"))
                .CataloguePrices.First();

            var addedSolution = new OrderItem
            {
                CataloguePrice = price,
                Price = 1.01M,
                DefaultDeliveryDate = order.CommencementDate,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                EstimationPeriod = TimeUnit.PerMonth,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99999, "001")),
            };

            var recipient = new ServiceRecipient
            {
                OdsCode = "03F",
                Name = "Test Recipient",
            };

            var orderItemRecipients = new List<OrderItemRecipient>
            {
                new()
                {
                    Recipient = recipient,
                    DeliveryDate = order.CommencementDate,
                    Quantity = 1000,
                },
            };

            addedSolution.SetRecipients(orderItemRecipients);

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            order.AddOrUpdateOrderItem(addedSolution);

            context.Add(order);
        }

        private static void AddOrderWithAddedNoContactCatalogueSolution(BuyingCatalogueDbContext context)
        {
            const int orderId = 90006;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = 99998,
                SupplierContact = new Contact
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
                CommencementDate = timeNow.AddDays(1),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            var price = context.CatalogueItems
                .Include(c => c.CataloguePrices).ThenInclude(s => s.PricingUnit)
                .Single(c => c.Id == new CatalogueItemId(99998, "002"))
                .CataloguePrices.First();

            var addedSolution = new OrderItem
            {
                CataloguePrice = price,
                Price = 1.01M,
                DefaultDeliveryDate = order.CommencementDate,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                EstimationPeriod = TimeUnit.PerMonth,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "002")),
            };

            var recipients = context.ServiceRecipients.ToList();

            var orderItemRecipients = new List<OrderItemRecipient>();

            foreach (var recipient in recipients)
            {
                orderItemRecipients
                    .Add(new()
                    {
                        Recipient = recipient,
                        DeliveryDate = order.CommencementDate,
                        Quantity = 1000,
                    });
            }

            addedSolution.SetRecipients(orderItemRecipients);

            order.AddOrUpdateOrderItem(addedSolution);

            context.Add(order);
        }

        private static void AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolution(BuyingCatalogueDbContext context)
        {
            const int orderId = 90007;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = 99998,
                SupplierContact = new Contact
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
                CommencementDate = timeNow.AddDays(1),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            var priceSinglePriceSolution = context.CatalogueItems
                .Include(c => c.CataloguePrices).ThenInclude(s => s.PricingUnit)
                .Single(c => c.Id == new CatalogueItemId(99998, "002"))
                .CataloguePrices.First();

            var priceMultiplePriceSolution = context.CatalogueItems
                .Include(c => c.CataloguePrices).ThenInclude(s => s.PricingUnit)
                .Single(c => c.Id == new CatalogueItemId(99998, "001"))
                .CataloguePrices.First();

            var additionalPrice = context.CatalogueItems
                .Include(c => c.CataloguePrices).ThenInclude(s => s.PricingUnit)
                .Single(c => c.Id == new CatalogueItemId(99998, "002A999"))
                .CataloguePrices.First();

            var addedSinglePriceCatalogueSolution = new OrderItem
            {
                CataloguePrice = priceSinglePriceSolution,
                Price = 1.01M,
                DefaultDeliveryDate = order.CommencementDate,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                EstimationPeriod = TimeUnit.PerMonth,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "002")),
            };

            var addedMultiplePriceCatalogueSolution = new OrderItem
            {
                CataloguePrice = priceMultiplePriceSolution,
                Price = 1.01M,
                DefaultDeliveryDate = order.CommencementDate,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                EstimationPeriod = TimeUnit.PerMonth,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            var addedAdditionalSolution = new OrderItem
            {
                CataloguePrice = additionalPrice,
                Price = 1.01M,
                DefaultDeliveryDate = order.CommencementDate,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                EstimationPeriod = TimeUnit.PerMonth,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "002A999")),
            };

            var recipients = context.ServiceRecipients.ToList();

            var orderItemRecipients = new List<OrderItemRecipient>();

            foreach (var recipient in recipients)
            {
                orderItemRecipients
                    .Add(new()
                    {
                        Recipient = recipient,
                        DeliveryDate = order.CommencementDate,
                        Quantity = 1000,
                    });
            }

            addedSinglePriceCatalogueSolution.SetRecipients(orderItemRecipients);

            addedMultiplePriceCatalogueSolution.SetRecipients(orderItemRecipients);

            addedAdditionalSolution.SetRecipients(orderItemRecipients);

            order.AddOrUpdateOrderItem(addedSinglePriceCatalogueSolution);

            order.AddOrUpdateOrderItem(addedMultiplePriceCatalogueSolution);

            order.AddOrUpdateOrderItem(addedAdditionalSolution);

            context.Add(order);
        }

        private static void AddOrderWithAddedAssociatedService(BuyingCatalogueDbContext context)
        {
            const int orderId = 90008;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = 99998,
                SupplierContact = new Contact
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
                CommencementDate = timeNow.AddDays(1),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            var price = context.CatalogueItems
                .Include(c => c.CataloguePrices).ThenInclude(s => s.PricingUnit)
                .Single(c => c.Id == new CatalogueItemId(99998, "S-999"))
                .CataloguePrices.First();

            var addedSolution = new OrderItem
            {
                CataloguePrice = price,
                Price = 1.01M,
                DefaultDeliveryDate = order.CommencementDate,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                EstimationPeriod = TimeUnit.PerMonth,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            order.AddOrUpdateOrderItem(addedSolution);

            context.Add(order);
        }

        private static void AddOrderReadyToComplete(BuyingCatalogueDbContext context)
        {
            const int orderId = 90009;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = 99998,
                SupplierContact = new Contact
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
                CommencementDate = timeNow.AddDays(1),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxx
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            var price = context.CatalogueItems
                .Include(c => c.CataloguePrices).ThenInclude(s => s.PricingUnit)
                .Single(c => c.Id == new CatalogueItemId(99998, "S-999"))
                .CataloguePrices.First();

            var addedSolution = new OrderItem
            {
                CataloguePrice = price,
                Price = 1.01M,
                DefaultDeliveryDate = order.CommencementDate,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                EstimationPeriod = TimeUnit.PerMonth,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            order.FundingSourceOnlyGms = true;

            order.AddOrUpdateOrderItem(addedSolution);

            context.Add(order);
        }

        private static void AddCompletedOrder(BuyingCatalogueDbContext context)
        {
            const int orderId = 90010;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.Incomplete,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
                    Phone = "123456789",
                },
                SupplierId = 99998,
                SupplierContact = new Contact
                {
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    Email = "bat.man@Gotham.Fake",
                    Phone = "123456789",
                },
                CommencementDate = timeNow.AddDays(1),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            // mjrxxxx
            // mjrxxxx URGENT - search for these commented out calls. Are they all in E2E? Need to run a test and break in context to see if these set statements are required at all
            //order.SetLastUpdatedBy(
            //    user.Id,
            //    $"{user.FirstName} {user.LastName}");
            order.LastUpdatedBy = user.Id;
            order.LastUpdated = DateTime.UtcNow;

            var price = context.CatalogueItems
                .Include(c => c.CataloguePrices).ThenInclude(s => s.PricingUnit)
                .Single(c => c.Id == new CatalogueItemId(99998, "S-999"))
                .CataloguePrices.First();

            var addedSolution = new OrderItem
            {
                CataloguePrice = price,
                Price = 1.01M,
                DefaultDeliveryDate = order.CommencementDate,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                EstimationPeriod = TimeUnit.PerMonth,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            order.FundingSourceOnlyGms = true;

            order.Complete();

            order.AddOrUpdateOrderItem(addedSolution);

            context.Add(order);
        }

        private static int GetOrganisationId(BuyingCatalogueDbContext context, string odsCode = "03F")
        {
            return context.Organisations.First(o => o.OdsCode == odsCode).Id;
        }

        private static AspNetUser GetBuyerUser(BuyingCatalogueDbContext context, int organisationId)
        {
            return context.Users.FirstOrDefault(u => u.OrganisationFunction == "Buyer" && u.PrimaryOrganisationId == organisationId);
        }
    }
}
