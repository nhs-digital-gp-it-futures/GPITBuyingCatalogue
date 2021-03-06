using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;

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
            AddOrderWithAddedCatalogueSolutionButNoData(context);
            AddOrderWithAddedCatalogueSolutionAndServicesButNoData(context);
            AddOrderWithAddedCatalogueSolution(context);
            AddOrderWithAddedCatalogueSolutionNoFundingRequired(context);
            AddOrderWithAddedNoContactCatalogueSolution(context);
            AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolution(context);
            AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolutionAndPrice(context);
            AddOrderWithAddedAssociatedService(context);
            AddOrderWithAddedAssociatedServiceAndOrderItemPrice(context);
            AddOrderWithAddedAssociatedServiceAndServiceRecipientPrice(context);
            AddOrderWithAddedNoContactSolutionAdditionalServiceAndAssociatedService(context);
            AddAssociatedServicesOnlyOrder(context);
            AddEmptyAssociatedServicesOnlyOrder(context);
            AddAssociatedServicesOnlyOrderWithNoPriceOrServiceRecipients(context);
            AddAssociatedServicesOnlyOrderWithOnePopulatedOrderItem(context);
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
                OrderTriageValue = OrderTriageValue.Over250K,
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

        private static void AddOrderWithAddedCatalogueSolutionButNoData(BuyingCatalogueDbContext context)
        {
            const int orderId = 90012;
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

            var addedSolution = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            order.OrderItems.Add(addedSolution);

            var user = GetBuyerUser(context, order.OrderingPartyId);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddOrderWithAddedCatalogueSolutionAndServicesButNoData(BuyingCatalogueDbContext context)
        {
            const int orderId = 91012;
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

            var solution = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            var additionalService = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001A99")),
            };

            var associatedService = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            order.OrderItems.Add(solution);
            order.OrderItems.Add(additionalService);
            order.OrderItems.Add(associatedService);

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
                InitialPeriod = 6,
                MaximumTerm = 36,
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
                Quantity = 10,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99999, "001")),
                OrderItemFunding = new OrderItemFunding
                {
                    CatalogueItemId = new CatalogueItemId(99998, "001"),
                    OrderId = orderId,
                    OrderItemFundingType = OrderItemFundingType.LocalFundingOnly,
                },
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddOrderWithAddedCatalogueSolutionNoFundingRequired(BuyingCatalogueDbContext context)
        {
            const int orderId = 90015;
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
                InitialPeriod = 6,
                MaximumTerm = 36,
            };

            var price = context.CatalogueItems
                    .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .Single();

            price.CataloguePriceCalculationType = EntityFramework.Catalogue.Models.CataloguePriceCalculationType.SingleFixed;
            price.OrderItemPriceTiers.First().Price = 0;

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99999, "001")),
                OrderItemFunding = new OrderItemFunding
                {
                    CatalogueItemId = new CatalogueItemId(99998, "001"),
                    OrderId = orderId,
                    OrderItemFundingType = OrderItemFundingType.NoFundingRequired,
                },
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
                InitialPeriod = 6,
                MaximumTerm = 36,
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
                Quantity = 10,
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
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001A99")),
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
            });

            order.OrderItems.Add(addedSinglePriceCatalogueSolution);
            order.OrderItems.Add(addedMultiplePriceCatalogueSolution);
            order.OrderItems.Add(addedAdditionalSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolutionAndPrice(BuyingCatalogueDbContext context)
        {
            const int orderId = 91007;
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
                OrderItemPrice = priceSinglePriceSolution,
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
            var solution = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001"));
            var additionalService = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999"));

            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = solution });
            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = additionalService });

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddOrderWithAddedAssociatedServiceAndOrderItemPrice(BuyingCatalogueDbContext context)
        {
            const int orderId = 91008;
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
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddOrderWithAddedAssociatedServiceAndServiceRecipientPrice(BuyingCatalogueDbContext context)
        {
            const int orderId = 92008;
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
                .Where(c => c.Id == new CatalogueItemId(99999, "001"))
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

            recipients.ForEach(r => addedSolution.OrderItemRecipients.Add(new OrderItemRecipient { Recipient = r }));

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
                OrderItemFundingType = OrderItemFundingType.CentralFunding,
            };

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddOrderWithAddedNoContactSolutionAdditionalServiceAndAssociatedService(BuyingCatalogueDbContext context)
        {
            const int orderId = 90013;
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
                InitialPeriod = 6,
                MaximumTerm = 36,
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

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

            var associatedPrice = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "S-997"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

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

            var addedAssociatedSolution = new OrderItem
            {
                OrderItemPrice = associatedPrice,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-997")),
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                var recipient = new OrderItemRecipient
                {
                    Recipient = r,
                    Quantity = 1000,
                };

                addedMultiplePriceCatalogueSolution.OrderItemRecipients.Add(recipient);
                addedAdditionalSolution.OrderItemRecipients.Add(recipient);
                addedAssociatedSolution.OrderItemRecipients.Add(recipient);
            });

            order.OrderItems.Add(addedMultiplePriceCatalogueSolution);
            order.OrderItems.Add(addedAdditionalSolution);
            order.OrderItems.Add(addedAssociatedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddAssociatedServicesOnlyOrder(BuyingCatalogueDbContext context)
        {
            const int orderId = 90014;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                AssociatedServicesOnly = true,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.InProgress,
                IsDeleted = false,
                Description = "Associated services only",
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
                InitialPeriod = 6,
                MaximumTerm = 36,
                SolutionId = new CatalogueItemId(99998, "001"),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "S-997"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .Single();

            var service = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-997")),
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                var recipient = new OrderItemRecipient
                {
                    Recipient = r,
                    Quantity = 1000,
                };

                service.OrderItemRecipients.Add(recipient);
            });

            order.OrderItems.Add(service);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddEmptyAssociatedServicesOnlyOrder(BuyingCatalogueDbContext context)
        {
            const int orderId = 90018;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                AssociatedServicesOnly = true,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.InProgress,
                IsDeleted = false,
                Description = "Associated services only",
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
                InitialPeriod = 6,
                MaximumTerm = 36,
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddAssociatedServicesOnlyOrderWithNoPriceOrServiceRecipients(BuyingCatalogueDbContext context)
        {
            const int orderId = 90016;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                AssociatedServicesOnly = true,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.InProgress,
                IsDeleted = false,
                Description = "Associated services only",
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
                InitialPeriod = 6,
                MaximumTerm = 36,
                SolutionId = new CatalogueItemId(99998, "001"),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            var service = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-997")),
            };

            order.OrderItems.Add(service);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddAssociatedServicesOnlyOrderWithOnePopulatedOrderItem(BuyingCatalogueDbContext context)
        {
            const int orderId = 90017;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                AssociatedServicesOnly = true,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                OrderStatus = OrderStatus.InProgress,
                IsDeleted = false,
                Description = "Associated services only",
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
                InitialPeriod = 6,
                MaximumTerm = 36,
                SolutionId = new CatalogueItemId(99998, "001"),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            var recipients = context.ServiceRecipients.Select(x => new OrderItemRecipient
            {
                Recipient = x,
                Quantity = 1,
            });

            order.OrderItems.Add(new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-997")),
                OrderItemRecipients = recipients.ToList(),
            });

            order.OrderItems.Add(new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-998")),
            });

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
                OrderItemFundingType = OrderItemFundingType.CentralFunding,
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
