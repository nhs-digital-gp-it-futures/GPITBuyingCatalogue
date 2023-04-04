using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public class OrderSummary : BuyerTestBase
    {
        private const string InternalOrgId = "CG-03F";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
        };

        public OrderSummary(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DashboardController),
                  nameof(DashboardController.Organisation),
                  Parameters)
        {
        }

        [Fact]
        public void SolutionAndServices_AllSectionsDisplayed()
        {
            var order = CreateOrder(
                CreateSolutionOrderItem(),
                CreateAdditionalServiceOrderItem(),
                CreateAssociatedServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                  typeof(OrderController),
                  nameof(OrderController.Summary),
                  parameters);

            CommonActions.PageTitle().Should().Be($"Review and complete your order summary - {order.CallOffId}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Review your order summary before completing it. Once the order summary is completed, you'll be unable to make changes.".FormatForComparison());

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.OrderIdSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.OrderDescriptionSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.DateCreatedSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.OrderingPartySummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SupplierSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.StartDateSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.InitialPeriodSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.MaximumTermSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.EndDateSummary).Should().BeTrue();

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SolutionSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AdditionalServicesSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AssociatedServicesSection).Should().BeTrue();

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.IndicativeCostsSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.OneOffCostSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.MonthlyCostSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.OneYearCostSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.TotalCostSummary).Should().BeTrue();

            CommonActions.ContinueButtonDisplayed().Should().BeTrue();

            RemoveOrder(order);
        }

        [Fact]
        public void SolutionAndAdditionalService_RelevantSectionsDisplayed()
        {
            var order = CreateOrder(
                CreateSolutionOrderItem(),
                CreateAdditionalServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                  typeof(OrderController),
                  nameof(OrderController.Summary),
                  parameters);

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SolutionSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AdditionalServicesSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AssociatedServicesSection).Should().BeFalse();

            RemoveOrder(order);
        }

        [Fact]
        public void SolutionAndAssociatedService_RelevantSectionsDisplayed()
        {
            var order = CreateOrder(
                CreateSolutionOrderItem(),
                CreateAssociatedServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                  typeof(OrderController),
                  nameof(OrderController.Summary),
                  parameters);

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SolutionSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AdditionalServicesSection).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AssociatedServicesSection).Should().BeTrue();

            RemoveOrder(order);
        }

        [Fact]
        public void SolutionOnly_RelevantSectionsDisplayed()
        {
            var order = CreateOrder(
                CreateSolutionOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                  typeof(OrderController),
                  nameof(OrderController.Summary),
                  parameters);

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SolutionSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AdditionalServicesSection).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AssociatedServicesSection).Should().BeFalse();

            RemoveOrder(order);
        }

        [Fact]
        public void AssociatedServiceOnly_RelevantSectionsDisplayed()
        {
            var order = CreateOrder(
                CreateAssociatedServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                  typeof(OrderController),
                  nameof(OrderController.Summary),
                  parameters);

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SolutionSection).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AdditionalServicesSection).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AssociatedServicesSection).Should().BeTrue();

            RemoveOrder(order);
        }

        [Fact]
        public void ClickGoBackLink_ExpectedResult()
        {
            var order = CreateOrder(
                CreateSolutionOrderItem(),
                CreateAdditionalServiceOrderItem(),
                CreateAssociatedServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                parameters);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            RemoveOrder(order);
        }

        private static OrderItemFunding CreateOrderItemFunding(OrderItem orderItem)
            => new()
            {
                OrderItem = orderItem,
                CatalogueItemId = orderItem.CatalogueItemId,
                OrderItemFundingType = OrderItemFundingType.CentralFunding,
            };

        private Order CreateOrder(
            params OrderItem[] orderItems)
        {
            int GetOrganisationId(BuyingCatalogueDbContext context, string internalOrgId = "CG-03F")
                => context.Organisations.First(o => o.InternalIdentifier == internalOrgId).Id;

            AspNetUser GetBuyerUser(BuyingCatalogueDbContext context, int organisationId)
                => GetUserByRole(OrganisationFunction.Buyer.Name).First(u => u.PrimaryOrganisationId == organisationId);

            using var context = GetEndToEndDbContext();
            var timeNow = DateTime.UtcNow;

            var order = new Order
            {
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
            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(r =>
            {
                OrderItemRecipient CreateRecipient(CatalogueItemId itemId)
                    => new()
                    {
                        Recipient = r,
                        Quantity = 1000,
                        CatalogueItemId = itemId,
                        DeliveryDate = timeNow.AddDays(2).Date,
                    };

                foreach (var orderItem in orderItems)
                    orderItem.OrderItemRecipients.Add(CreateRecipient(orderItem.CatalogueItemId));
            });

            foreach (var orderItem in orderItems)
            {
                orderItem.OrderItemFunding = CreateOrderItemFunding(orderItem);
                order.OrderItems.Add(orderItem);
            }

            context.Add(order);
            context.SaveChangesAs(user.Id);

            return order;
        }

        private OrderItem CreateSolutionOrderItem(OrderItemPrice price = null)
        {
            var addedSolution = new OrderItem
            {
                OrderItemPrice = price ?? GetFlatPrice(),
                Created = DateTime.UtcNow,
                CatalogueItemId = new CatalogueItemId(99998, "001"),
            };

            return addedSolution;
        }

        private OrderItem CreateAdditionalServiceOrderItem(OrderItemPrice price = null)
        {
            var addedAdditionalService = new OrderItem
            {
                OrderItemPrice = price ?? GetFlatPrice(),
                Created = DateTime.UtcNow,
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
            };

            return addedAdditionalService;
        }

        private OrderItem CreateAssociatedServiceOrderItem(OrderItemPrice price = null)
        {
            var addedAssociatedService = new OrderItem
            {
                OrderItemPrice = price ?? GetFlatPrice(),
                Created = DateTime.UtcNow,
                CatalogueItemId = new CatalogueItemId(99998, "S-999"),
            };

            return addedAssociatedService;
        }

        private OrderItemPrice GetFlatPrice()
        {
            var flatPrice = GetPrice(CataloguePriceType.Flat);

            return new OrderItemPrice(flatPrice);
        }

        private CataloguePrice GetPrice(CataloguePriceType priceType)
        {
            using var context = GetEndToEndDbContext();

            return context
                .CataloguePrices
                .Include(cp => cp.CataloguePriceTiers)
                .Include(cp => cp.PricingUnit)
                .FirstOrDefault(cp =>
                    cp.CataloguePriceType == priceType);
        }

        private void RemoveOrder(Order order)
        {
            using var context = GetEndToEndDbContext();
            var dbOrder = context.Orders.AsNoTracking().First(x => x.Id == order.Id);

            context.Orders.Remove(dbOrder);
            context.SaveChanges();
        }
    }
}
