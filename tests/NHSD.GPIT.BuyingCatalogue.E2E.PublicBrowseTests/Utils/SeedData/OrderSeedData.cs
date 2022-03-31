using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
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
            AddOrderAtSupplierContactStage(context);
            AddOrderAtCommencementDateStage(context);
            AddOrderAtCatalogueSolutionStage(context);
            AddOrderWithAddedCatalogueSolution(context);
            AddOrderWithAddedNoContactCatalogueSolution(context);
            AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolution(context);
            AddOrderWithAddedAssociatedService(context);
            AddOrderReadyToComplete(context);
            AddCompletedOrder(context, 90010, GetOrganisationId(context));
            AddCompletedOrder(context, 90011, GetOrganisationId(context, "CG-15F"));
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
                OrderStatus = OrderStatus.InProgress,
                IsDeleted = false,
                Description = "This is an Order Description",
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
                IsDeleted = false,
                Description = "This is an Order Description",
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
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

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddOrderAtSupplierContactStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 91002;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.InProgress,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.com",
                    Phone = "123456789",
                },
                SupplierId = 99998,
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
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

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
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

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
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
                    .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .Single();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99999, "001")),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
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

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "002")),
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                addedSolution.OrderItemRecipients.Add(new()
                {
                    Recipient = r,
                    Quantity = 1000,
                });
            });

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
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

            var priceSinglePriceSolution = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

            var priceMultiplePriceSolution = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "001"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

            var additionalPrice = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "002A999"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

            var addedSinglePriceCatalogueSolution = new OrderItem
            {
                OrderItemPrice = priceSinglePriceSolution,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "002")),
            };

            var addedMultiplePriceCatalogueSolution = new OrderItem
            {
                OrderItemPrice = priceMultiplePriceSolution,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            var addedAdditionalSolution = new OrderItem
            {
                OrderItemPrice = additionalPrice,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "002A999")),
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                var recipient = new OrderItemRecipient
                {
                    Recipient = r,
                    Quantity = 1000,
                };

                addedSinglePriceCatalogueSolution.OrderItemRecipients.Add(recipient);
                addedMultiplePriceCatalogueSolution.OrderItemRecipients.Add(recipient);
                addedAdditionalSolution.OrderItemRecipients.Add(recipient);
            });

            order.OrderItems.Add(addedSinglePriceCatalogueSolution);
            order.OrderItems.Add(addedMultiplePriceCatalogueSolution);
            order.OrderItems.Add(addedAdditionalSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
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

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "S-999"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
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
                OrderStatus = OrderStatus.InProgress,
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

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "S-999"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                var recipient = new OrderItemRecipient
                {
                    Recipient = r,
                    Quantity = 1000,
                };

                addedSolution.OrderItemRecipients.Add(recipient);
            });

            addedSolution.OrderItemFunding = new OrderItemFunding
            {
                OrderId = addedSolution.OrderId,
                CatalogueItemId = addedSolution.CatalogueItemId,
                TotalPrice = addedSolution.CalculateTotalCost(),
                CentralAllocation = addedSolution.CalculateTotalCost(),
                LocalAllocation = 0,
            };

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddCompletedOrder(BuyingCatalogueDbContext context, int orderId, int organisationId)
        {
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = organisationId,
                Created = timeNow,
                OrderStatus = OrderStatus.InProgress,
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

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "001"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                var recipient = new OrderItemRecipient
                {
                    Recipient = r,
                    Quantity = 1000,
                };

                addedSolution.OrderItemRecipients.Add(recipient);
            });

            addedSolution.OrderItemFunding = new OrderItemFunding
            {
                OrderId = addedSolution.OrderId,
                CatalogueItemId = addedSolution.CatalogueItemId,
                TotalPrice = addedSolution.CalculateTotalCost(),
                CentralAllocation = addedSolution.CalculateTotalCost(),
                LocalAllocation = 0,
            };

            order.Complete();

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static int GetOrganisationId(BuyingCatalogueDbContext context, string internalOrgId = "CG-03F")
        {
            return context.Organisations.First(o => o.InternalIdentifier == internalOrgId).Id;
        }

        private static AspNetUser GetBuyerUser(BuyingCatalogueDbContext context, int organisationId)
        {
            return context.Users.FirstOrDefault(u => u.OrganisationFunction == "Buyer" && u.PrimaryOrganisationId == organisationId);
        }
    }
}
