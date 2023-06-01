using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    internal class OrderSeedData : ISeedData
    {
        private const string DFOCVC = "DFOCVC001";
        private const string GPITFUTURES = "NHSDGP001";

        public static async Task Initialize(BuyingCatalogueDbContext context)
        {
            var orders = new List<Order>
            {
                AddOrderAtDescriptionStage(context),
                AddOrderAtCallOffPartyStage(context),
                AddOrderAtSupplierStage(context),
                AddOrderAtSupplierContactStage(context),
                AddOrderAtCommencementDateStage(context),
                AddOrderAtCatalogueSolutionStage(context),
                AddOrderWithAddedCatalogueSolutionButNoData(context),
                AddOrderWithAddedCatalogueSolutionAndServicesButNoData(context),
                AddOrderWithAddedCatalogueSolution(context),
                AddOrderWithAddedCatalogueSolutionNoSelectedFrameworkMultipleFrameworks(context),
                AddOrderWithAddedCatalogueSolutionSelectedFrameworkMultipleFrameworks(context),
                AddOrderWithAddedCatalogueSolutionNoSelectedFrameworkSingleFramework(context),
                AddOrderWithAddedCatalogueSolutionNoFundingRequired(context),
                AddOrderWithAddedNoContactCatalogueSolution(context),
                AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolution(context),
                AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolutionAndPrice(context),
                AddOrderWithAddedAssociatedService(context),
                AddOrderWithAddedAssociatedServiceAndOrderItemPrice(context),
                AddOrderWithAddedAssociatedServiceAndServiceRecipientPrice(context),
                AddOrderWithAddedNoContactSolutionAdditionalServiceAndAssociatedService(context),
                AddOrderWithMixtureOfServicesAndMatchingPlannedDeliveryDates(context),
                AddOrderWithMixtureOfServicesAndDifferingPlannedDeliveryDates(context),
                AddAssociatedServicesOnlyOrder(context),
                AddEmptyAssociatedServicesOnlyOrder(context),
                AddEmptyAssociatedServicesOnlyOrderNoSupplier(context),
                AddAssociatedServicesOnlyOrderWithNoPriceOrServiceRecipients(context),
                AddAssociatedServicesOnlyOrderWithOnePopulatedOrderItem(context),
                AddOrderReadyToComplete(context),
                AddCompletedOrder(context, 90010, GetOrganisationId(context)),
                AddCompletedOrder(context, 90011, GetOrganisationId(context, "CG-15F")),
                AddCompletedOrder(context, 90030, GetOrganisationId(context)),
                AddCompletedOrder(context, 90031, GetOrganisationId(context)),
                AddCompletedOrder(context, 90032, GetOrganisationId(context)),
                AddOrderByAccountManager(context),
            };

            await context.InsertRangeWithIdentityAsync(orders);

            var amendments = new[]
            {
                AddAmendment(context, orders.First(o => o.OrderNumber == 90030), 2),
                AddAmendment(context, orders.First(o => o.OrderNumber == 90031), 2),
                AddAmendment(context, orders.First(o => o.OrderNumber == 90032), 2),
                AddAmendment(context, orders.First(o => o.OrderNumber == 90033), 2),
            };

            context.AddRange(amendments);
            await context.SaveChangesAsync();

            AddOrderItemToOrder(context, 90030, 2, new CatalogueItemId(99998, "001"));
            AddOrderItemToOrder(context, 90031, 1, new CatalogueItemId(99998, "001A99"));
            AddOrderItemToOrder(context, 90031, 2, new CatalogueItemId(99998, "001A99"));
            AddOrderItemWithPriceAndRecipientsToOrder(context, 90032, 2, new CatalogueItemId(99998, "001A99"));
            AddOrderItemWithPriceAndRecipientsToOrder(context, 90032, 2, new CatalogueItemId(99998, "S-999"));
            AddOrderItemWithPriceAndRecipientsToOrder(context, 90033, 2, new CatalogueItemId(99999, "003"));

            await context.SaveChangesAsync();
        }

        private static Order AddOrderAtDescriptionStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90000;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
                Created = timeNow,
                IsDeleted = false,
                Description = "This is an Order Description",
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static Order AddOrderAtCallOffPartyStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90001;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
                Created = timeNow,
                IsDeleted = false,
                Description = "This is an Order Description",
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static Order AddOrderAtSupplierStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90002;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static Order AddOrderAtSupplierContactStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 91002;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static Order AddOrderAtCommencementDateStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90003;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static Order AddOrderAtCatalogueSolutionStage(BuyingCatalogueDbContext context)
        {
            const int orderId = 90004;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static Order AddOrderWithAddedCatalogueSolutionButNoData(BuyingCatalogueDbContext context)
        {
            const int orderId = 90012;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var addedSolution = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddOrderWithAddedCatalogueSolutionAndServicesButNoData(BuyingCatalogueDbContext context)
        {
            const int orderId = 91012;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var solution = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            var additionalService = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001A99")),
            };

            var associatedService = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            order.OrderItems.Add(solution);
            order.OrderItems.Add(additionalService);
            order.OrderItems.Add(associatedService);

            return order;
        }

        private static Order AddOrderWithAddedCatalogueSolution(BuyingCatalogueDbContext context)
        {
            const int orderId = 90005;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                SelectedFramework = GetFramework(context, DFOCVC),
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                    .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99999, "001")),
                OrderItemFunding = new OrderItemFunding
                {
                    CatalogueItemId = new CatalogueItemId(99999, "001"),
                    OrderId = orderId,
                    OrderItemFundingType = OrderItemFundingType.LocalFundingOnly,
                    LastUpdatedBy = user.Id,
                },
                LastUpdatedBy = user.Id,
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

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddOrderWithAddedCatalogueSolutionNoSelectedFrameworkMultipleFrameworks(BuyingCatalogueDbContext context)
        {
            const int orderId = 90020;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                    .Where(c => c.Id == new CatalogueItemId(99999, "003"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99999, "003")),
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

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddOrderWithAddedCatalogueSolutionSelectedFrameworkMultipleFrameworks(BuyingCatalogueDbContext context)
        {
            const int orderId = 90033;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                SelectedFramework = GetFramework(context, GPITFUTURES),
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                    .Where(c => c.Id == new CatalogueItemId(99999, "003"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99999, "003")),
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

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddOrderWithAddedCatalogueSolutionNoSelectedFrameworkSingleFramework(BuyingCatalogueDbContext context)
        {
            const int orderId = 90021;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                    .Where(c => c.Id == new CatalogueItemId(99999, "001"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99999, "001")),
            };

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddOrderWithAddedCatalogueSolutionNoFundingRequired(BuyingCatalogueDbContext context)
        {
            const int orderId = 90015;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                SelectedFramework = GetFramework(context, GPITFUTURES),
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                    .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                    .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                    .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                    .First();

            price.CataloguePriceCalculationType = EntityFramework.Catalogue.Models.CataloguePriceCalculationType.SingleFixed;
            price.OrderItemPriceTiers.First().Price = 0;

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99999, "001")),
                OrderItemFunding = new OrderItemFunding
                {
                    CatalogueItemId = new CatalogueItemId(99999, "001"),
                    OrderId = orderId,
                    OrderItemFundingType = OrderItemFundingType.NoFundingRequired,
                    LastUpdatedBy = user.Id,
                },
            };

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddOrderWithAddedNoContactCatalogueSolution(BuyingCatalogueDbContext context)
        {
            const int orderId = 90006;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                CommencementDate = timeNow.AddMonths(1),
                InitialPeriod = 6,
                MaximumTerm = 36,
                SelectedFramework = GetFramework(context, GPITFUTURES),
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                Quantity = 10,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "002")),
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

            return order;
        }

        private static Order AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolution(BuyingCatalogueDbContext context)
        {
            const int orderId = 90007;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var priceSinglePriceSolution = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var priceMultiplePriceSolution = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "001"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var addedSinglePriceCatalogueSolution = new OrderItem
            {
                OrderItemPrice = priceSinglePriceSolution,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "002")),
            };

            var addedMultiplePriceCatalogueSolution = new OrderItem
            {
                OrderItemPrice = priceMultiplePriceSolution,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            var addedAdditionalSolution = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001A99")),
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

            return order;
        }

        private static Order AddOrderWithAddedNoContactSolutionAndNoContactAdditionalSolutionAndPrice(BuyingCatalogueDbContext context)
        {
            const int orderId = 91007;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var priceSinglePriceSolution = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var priceMultiplePriceSolution = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "001"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var addedSinglePriceCatalogueSolution = new OrderItem
            {
                OrderItemPrice = priceSinglePriceSolution,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "002")),
            };

            var addedMultiplePriceCatalogueSolution = new OrderItem
            {
                OrderItemPrice = priceMultiplePriceSolution,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            var addedAdditionalSolution = new OrderItem
            {
                OrderItemPrice = priceSinglePriceSolution,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "002A999")),
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

            return order;
        }

        private static Order AddOrderWithAddedAssociatedService(BuyingCatalogueDbContext context)
        {
            const int orderId = 90008;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var solution = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001"));
            var additionalService = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-999"));

            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = solution });
            order.OrderItems.Add(new OrderItem { Created = DateTime.UtcNow, OrderId = orderId, CatalogueItem = additionalService });

            return order;
        }

        private static Order AddOrderWithAddedAssociatedServiceAndOrderItemPrice(BuyingCatalogueDbContext context)
        {
            const int orderId = 91008;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "002"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddOrderWithAddedAssociatedServiceAndServiceRecipientPrice(BuyingCatalogueDbContext context)
        {
            const int orderId = 92008;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99999, "001"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-999")),
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r => addedSolution.OrderItemRecipients.Add(new OrderItemRecipient { Recipient = r }));

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddOrderWithAddedNoContactSolutionAdditionalServiceAndAssociatedService(BuyingCatalogueDbContext context)
        {
            const int orderId = 90013;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var priceMultiplePriceSolution = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "001"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var additionalPrice = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "002A999"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var associatedPrice = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "S-997"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var addedMultiplePriceCatalogueSolution = new OrderItem
            {
                OrderItemPrice = priceMultiplePriceSolution,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001")),
            };

            var addedAdditionalSolution = new OrderItem
            {
                OrderItemPrice = additionalPrice,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "002A999")),
            };

            var addedAssociatedSolution = new OrderItem
            {
                OrderItemPrice = associatedPrice,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-997")),
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

            return order;
        }

        private static Order AddOrderWithMixtureOfServicesAndMatchingPlannedDeliveryDates(BuyingCatalogueDbContext context)
        {
            const int orderId = 90022;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                InitialPeriod = 3,
                MaximumTerm = 12,
                DeliveryDate = timeNow.AddDays(3).Date,
                LastUpdatedBy = user.Id,
            };

            var solution = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001"));
            var additionalService = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001A99"));
            var associatedService = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-999"));

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

            return order;
        }

        private static Order AddOrderWithMixtureOfServicesAndDifferingPlannedDeliveryDates(BuyingCatalogueDbContext context)
        {
            const int orderId = 90023;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var solution = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001"));
            var additionalService = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001A99"));
            var associatedService = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-999"));

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

            return order;
        }

        private static Order AddOrderReadyToComplete(BuyingCatalogueDbContext context)
        {
            const int orderId = 90009;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
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
                SelectedFramework = GetFramework(context, GPITFUTURES),
                LastUpdatedBy = user.Id,
                ContractFlags = new ContractFlags
                {
                    UseDefaultImplementationPlan = false,
                    UseDefaultDataProcessing = false,
                },
            };

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "001"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001")),
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
                LastUpdatedBy = user.Id,
            };

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static Order AddAssociatedServicesOnlyOrder(BuyingCatalogueDbContext context)
        {
            const int orderId = 90014;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                AssociatedServicesOnly = true,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "S-997"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var service = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-997")),
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

            return order;
        }

        private static Order AddEmptyAssociatedServicesOnlyOrder(BuyingCatalogueDbContext context)
        {
            const int orderId = 90018;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                AssociatedServicesOnly = true,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static Order AddEmptyAssociatedServicesOnlyOrderNoSupplier(BuyingCatalogueDbContext context)
        {
            const int orderId = 90019;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                AssociatedServicesOnly = true,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static Order AddAssociatedServicesOnlyOrderWithNoPriceOrServiceRecipients(BuyingCatalogueDbContext context)
        {
            const int orderId = 90016;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                AssociatedServicesOnly = true,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var service = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-997")),
            };

            order.OrderItems.Add(service);

            return order;
        }

        private static Order AddAssociatedServicesOnlyOrderWithOnePopulatedOrderItem(BuyingCatalogueDbContext context)
        {
            const int orderId = 90017;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context);
            var user = GetBuyerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                AssociatedServicesOnly = true,
                OrderingPartyId = organisation,
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
                LastUpdatedBy = user.Id,
            };

            var recipients = context.ServiceRecipients.Select(x => new OrderItemRecipient
            {
                Recipient = x,
                Quantity = 1,
            });

            order.OrderItems.Add(new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-997")),
                OrderItemRecipients = recipients.ToList(),
            });

            order.OrderItems.Add(new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "S-998")),
            });

            return order;
        }

        private static Order AddCompletedOrder(BuyingCatalogueDbContext context, int orderId, int organisationId)
        {
            var timeNow = DateTime.UtcNow;
            var user = GetBuyerUser(context, organisationId);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
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
                CommencementDate = timeNow.AddMonths(1),
                InitialPeriod = 6,
                MaximumTerm = 12,
                OrderTriageValue = OrderTriageValue.Under40K,
                LastUpdatedBy = user.Id,
            };

            var price = context.CatalogueItems
                .Where(c => c.Id == new CatalogueItemId(99998, "001"))
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                .First();

            var addedSolution = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = orderId,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == new CatalogueItemId(99998, "001")),
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
                LastUpdatedBy = user.Id,
            };

            order.Complete();

            order.OrderItems.Add(addedSolution);

            return order;
        }

        private static void AddOrderItemToOrder(BuyingCatalogueDbContext context, int orderNumber, int revision, CatalogueItemId catalogueItemId)
        {
            var order = context.Orders.First(x => x.OrderNumber == orderNumber && x.Revision == revision);

            var orderItem = new OrderItem
            {
                Created = DateTime.UtcNow,
                OrderId = order.Id,
                CatalogueItem = context.CatalogueItems.First(c => c.Id == catalogueItemId),
            };

            order.OrderItems.Add(orderItem);
        }

        private static void AddOrderItemWithPriceAndRecipientsToOrder(BuyingCatalogueDbContext context, int orderNumber, int revision, CatalogueItemId catalogueItemId)
        {
            var order = context.Orders.First(x => x.OrderNumber == orderNumber && x.Revision == revision);

            var catalogueItem = context.CatalogueItems.First(c => c.Id == catalogueItemId);

            var price = context.CatalogueItems
                            .Where(c => c.Id == catalogueItemId)
                            .Include(c => c.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                            .Include(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                            .Select(ci => new OrderItemPrice(ci.CataloguePrices.First()))
                            .First();

            var orderItem = new OrderItem
            {
                OrderItemPrice = price,
                Created = DateTime.UtcNow,
                OrderId = order.Id,
                CatalogueItem = catalogueItem,
            };

            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                var recipient = new OrderItemRecipient
                {
                    Recipient = r,
                    Quantity = 1000,
                    DeliveryDate = DateTime.UtcNow.AddDays(1),
                };

                orderItem.OrderItemRecipients.Add(recipient);
            });

            order.OrderItems.Add(orderItem);
        }

        private static Order AddAmendment(BuyingCatalogueDbContext context, Order order, int revision)
        {
            var user = GetBuyerUser(context, GetOrganisationId(context));
            var amendedOrder = order.BuildAmendment(revision);
            amendedOrder.LastUpdatedBy = user.Id;
            amendedOrder.Created = DateTime.UtcNow;
            return amendedOrder;
        }

        private static Order AddOrderByAccountManager(BuyingCatalogueDbContext context)
        {
            const int orderId = 95000;
            var timeNow = DateTime.UtcNow;
            var organisation = GetOrganisationId(context, "CG-15H");
            var user = GetAccountManagerUser(context, organisation);

            var order = new Order
            {
                Id = orderId,
                OrderNumber = orderId,
                Revision = 1,
                OrderingPartyId = organisation,
                Created = timeNow,
                IsDeleted = false,
                Description = "This is an Order Description",
                LastUpdatedBy = user.Id,
            };

            return order;
        }

        private static int GetOrganisationId(BuyingCatalogueDbContext context, string internalOrgId = "CG-03F")
        {
            return context.Organisations.First(o => o.InternalIdentifier == internalOrgId).Id;
        }

        private static AspNetUser GetBuyerUser(BuyingCatalogueDbContext context, int organisationId)
        {
            return GetUser(context, organisationId, OrganisationFunction.Buyer.Name);
        }

        private static AspNetUser GetAccountManagerUser(BuyingCatalogueDbContext context, int organisationId)
        {
            return GetUser(context, organisationId, OrganisationFunction.AccountManager.Name);
        }

        private static AspNetUser GetUser(BuyingCatalogueDbContext context, int organisationId, string role)
        {
            var users = context.Users.Include(u => u.AspNetUserRoles).ThenInclude(r => r.Role).Where(u => u.PrimaryOrganisationId == organisationId);

            var user = users.FirstOrDefault(
                u => u.AspNetUserRoles.Any(r => r.Role.Name == role));

            return user;
        }

        private static EntityFramework.Catalogue.Models.Framework GetFramework(BuyingCatalogueDbContext context, string frameworkId)
        {
            return context.Frameworks.First(f => f.Id == frameworkId);
        }
    }
}
