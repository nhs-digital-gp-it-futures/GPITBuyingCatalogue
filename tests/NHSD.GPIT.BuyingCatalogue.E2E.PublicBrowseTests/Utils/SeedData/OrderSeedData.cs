using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    internal static class OrderSeedData
    {
        private const string DFOCVC = "DFOCVC001";
        private const string GPITFUTURES = "NHSDGP001";

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
            AddOrderWithAddedCatalogueSolutionNoSelectedFrameworkMultipleFrameworks(context);
            AddOrderWithAddedCatalogueSolutionNoSelectedFrameworkSingleFramework(context);
            AddOrderWithAddedCatalogueSolutionNoFundingRequired(context);
            AddOrderWithAddedNoContactCatalogueSolution(context);
            AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolution(context);
            AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolutionAndPrice(context);
            AddOrderWithAddedAssociatedService(context);
            AddOrderWithAddedAssociatedServiceAndOrderItemPrice(context);
            AddOrderWithAddedAssociatedServiceAndServiceRecipientPrice(context);
            AddOrderWithAddedNoContactSolutionAdditionalServiceAndAssociatedService(context);
            AddOrderWithMixtureOfServicesAndMatchingPlannedDeliveryDates(context);
            AddOrderWithMixtureOfServicesAndDifferingPlannedDeliveryDates(context);
            AddAssociatedServicesOnlyOrder(context);
            AddEmptyAssociatedServicesOnlyOrder(context);
            AddEmptyAssociatedServicesOnlyOrderNoSupplier(context);
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

        private static async void AddOrderWithAddedCatalogueSolution(BuyingCatalogueDbContext context)
        {
            const int orderId = 90005;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
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
                SelectedFramework = await GetFramework(context, DFOCVC),
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
                    CatalogueItemId = new CatalogueItemId(99999, "001"),
                    OrderId = orderId,
                    OrderItemFundingType = OrderItemFundingType.LocalFundingOnly,
                },
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

            var user = GetBuyerUser(context, order.OrderingPartyId);

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static async void AddOrderWithAddedCatalogueSolutionNoSelectedFrameworkMultipleFrameworks(BuyingCatalogueDbContext context)
        {
            const int orderId = 90020;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
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
                DeliveryDate = DateTime.Today.AddDays(2),
                InitialPeriod = 6,
                MaximumTerm = 36,
            };

            var price = await context.CatalogueItems
                    .Where(c => c.Id == new CatalogueItemId(99999, "003"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .SingleAsync();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = await context.CatalogueItems.SingleAsync(c => c.Id == new CatalogueItemId(99999, "003")),
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                var recipient = new OrderItemRecipient
                {
                    Recipient = r,
                    Quantity = 1000,
                    DeliveryDate = DateTime.Today.AddDays(2),
                };

                addedSolution.OrderItemRecipients.Add(recipient);
            });

            var user = GetBuyerUser(context, order.OrderingPartyId);

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static async void AddOrderWithAddedCatalogueSolutionNoSelectedFrameworkSingleFramework(BuyingCatalogueDbContext context)
        {
            const int orderId = 90021;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
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

            var price = await context.CatalogueItems
                    .Where(c => c.Id == new CatalogueItemId(99999, "001"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .SingleAsync();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = await context.CatalogueItems.SingleAsync(c => c.Id == new CatalogueItemId(99999, "001")),
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static async void AddOrderWithAddedCatalogueSolutionNoFundingRequired(BuyingCatalogueDbContext context)
        {
            const int orderId = 90015;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
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
                SelectedFramework = await GetFramework(context, GPITFUTURES),
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
                    CatalogueItemId = new CatalogueItemId(99999, "001"),
                    OrderId = orderId,
                    OrderItemFundingType = OrderItemFundingType.NoFundingRequired,
                },
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);

            order.OrderItems.Add(addedSolution);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static async void AddOrderWithAddedNoContactCatalogueSolution(BuyingCatalogueDbContext context)
        {
            const int orderId = 90006;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
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
                SelectedFramework = await GetFramework(context, GPITFUTURES),
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
                CommencementDate = timeNow.AddDays(1).Date,
                DeliveryDate = timeNow.AddDays(2).Date,
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
                    DeliveryDate = timeNow.AddDays(1).Date,
                };

                addedSinglePriceCatalogueSolution.OrderItemRecipients.Add(recipient);
                addedMultiplePriceCatalogueSolution.OrderItemRecipients.Add(recipient);

                addedAdditionalSolution.OrderItemRecipients.Add(new OrderItemRecipient
                {
                    Recipient = r,
                    Quantity = 1000,
                    DeliveryDate = timeNow.AddDays(2).Date,
                });
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

        private static void AddOrderWithMixtureOfServicesAndMatchingPlannedDeliveryDates(BuyingCatalogueDbContext context)
        {
            const int orderId = 90022;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
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
                CommencementDate = timeNow.AddDays(2).Date,
                DeliveryDate = timeNow.AddDays(3).Date,
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);
            var solution = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001"));
            var additionalService = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001A99"));
            var associatedService = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999"));

            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = solution });
            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = additionalService });
            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = associatedService });

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r => order.OrderItems.ToList().ForEach(x => x.OrderItemRecipients.Add(new OrderItemRecipient
            {
                Recipient = r,
                OdsCode = r.OdsCode,
                DeliveryDate = timeNow.AddDays(3).Date,
            })));

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static void AddOrderWithMixtureOfServicesAndDifferingPlannedDeliveryDates(BuyingCatalogueDbContext context)
        {
            const int orderId = 90023;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
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
                CommencementDate = timeNow.AddDays(2).Date,
                DeliveryDate = timeNow.AddDays(3).Date,
            };

            var user = GetBuyerUser(context, order.OrderingPartyId);
            var solution = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001"));
            var additionalService = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "001A99"));
            var associatedService = context.CatalogueItems.Single(c => c.Id == new CatalogueItemId(99998, "S-999"));

            var solutionItem = new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = solution };

            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = additionalService });
            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = associatedService });

            context.ServiceRecipients.ForEach(r =>
            {
                order.OrderItems.ToList().ForEach(x => x.OrderItemRecipients.Add(new OrderItemRecipient
                {
                    Recipient = r,
                    DeliveryDate = timeNow.AddDays(3).Date,
                }));

                solutionItem.OrderItemRecipients.Add(new OrderItemRecipient
                {
                    Recipient = r,
                    DeliveryDate = timeNow.AddDays(2).Date,
                });
            });

            order.OrderItems.Add(solutionItem);

            context.Add(order);

            context.SaveChangesAs(user.Id);
        }

        private static async void AddOrderReadyToComplete(BuyingCatalogueDbContext context)
        {
            const int orderId = 90009;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
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
                CommencementDate = timeNow.AddDays(1).Date,
                DeliveryDate = timeNow.AddDays(1).Date,
                SelectedFramework = await GetFramework(context, GPITFUTURES),
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
                    DeliveryDate = timeNow.AddDays(1).Date,
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

        private static void AddEmptyAssociatedServicesOnlyOrderNoSupplier(BuyingCatalogueDbContext context)
        {
            const int orderId = 90019;
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
                Id = orderId,
                AssociatedServicesOnly = true,
                OrderingPartyId = GetOrganisationId(context),
                Created = timeNow,
                IsDeleted = false,
                Description = "Associated services only",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.Fake",
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
                SelectedFrameworkId = DFOCVC,
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
            var users = context.Users.Include(u => u.AspNetUserRoles).ThenInclude(r => r.Role).Where(u => u.PrimaryOrganisationId == organisationId);

            var user = users.FirstOrDefault(
                u => u.AspNetUserRoles.Any(r => r.Role.Name == OrganisationFunction.BuyerName));

            return user;
        }

        private static async Task<EntityFramework.Catalogue.Models.Framework> GetFramework(BuyingCatalogueDbContext context, string frameworkId)
        {
            return await context.Frameworks.SingleAsync(f => f.Id == frameworkId);
        }
    }
}
