using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Files;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Extensions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Polly;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public class OrderSummary : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90009;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

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
                false,
                CreateSolutionOrderItem(),
                CreateAdditionalServiceOrderItem(),
                CreateAssociatedServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                parameters);

            CommonActions.PageTitle().Should().Be($"Review order summary - {order.CallOffId}".FormatForComparison());
            CommonActions.LedeText()
                .Should()
                .Be("Review the items you’ve added to your order before completing it.".FormatForComparison());

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.OrderIdSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.OrderDescriptionSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.DateCreatedSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.OrderingPartySummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SupplierSummary).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SupplierLegalName).Should().BeTrue();
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

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.ImplementationPlanExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.BespokeImplementationPlan).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.AssociatedServicesExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.DataProcessingExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.BespokeDataProcessing).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.DownloadPdfButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.SaveForLaterButton).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            RemoveOrder(order);
        }

        [Fact]
        public void SolutionAndAdditionalService_RelevantSectionsDisplayed()
        {
            var order = CreateOrder(
                false,
                CreateSolutionOrderItem(),
                CreateAdditionalServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), order.CallOffId.ToString() },
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
                false,
                CreateSolutionOrderItem(),
                CreateAssociatedServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), order.CallOffId.ToString() },
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
                false,
                CreateSolutionOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), order.CallOffId.ToString() },
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
                true,
                CreateAssociatedServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), order.CallOffId.ToString() },
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
                false,
                CreateSolutionOrderItem(),
                CreateAdditionalServiceOrderItem(),
                CreateAssociatedServiceOrderItem());

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), order.CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                parameters);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.Order))
                .Should()
                .BeTrue();

            RemoveOrder(order);
        }

        [Fact]
        public void DefaultImplementationPlan_AllSectionsDisplayed()
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                parameters);

            var context = GetEndToEndDbContext();
            var flags = context.GetContractFlags(OrderId);

            flags.UseDefaultImplementationPlan = true;

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.ImplementationPlanExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.BespokeImplementationPlan).Should().BeFalse();

            flags.UseDefaultImplementationPlan = false;

            context.SaveChanges();
        }

        [Fact]
        public void DefaultDataProcessing_AllSectionsDisplayed()
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                parameters);

            var context = GetEndToEndDbContext();
            var flags = context.GetContractFlags(OrderId);

            flags.UseDefaultDataProcessing = true;

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.DataProcessingExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderSummaryObjects.BespokeDataProcessing).Should().BeFalse();

            flags.UseDefaultDataProcessing = false;

            context.SaveChanges();
        }

        [Fact]
        public void ClickSaveForLaterButton_ExpectedResult()
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                parameters);

            CommonActions.ClickLinkElement(OrderSummaryObjects.SaveForLaterButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DashboardController),
                    nameof(DashboardController.Organisation))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ClickCompleteOrder_ExpectedResult()
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                parameters);

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.Completed))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ClickDownloadPdfButton_ExpectedResult()
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                parameters);

            string filePath = @$"{Path.GetTempPath()}order-summary-in-progress-C0{OrderId}-01.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            CommonActions.ClickLinkElement(OrderSummaryObjects.DownloadPdfButton);

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }

        [Fact]
        public void OrderNotReadyToComplete_ClickContinue_ExpectedResult()
        {
            const int orderId = 90001;

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                Parameters2(orderId));

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.CompleteOrderButton).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.ContinueButton).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.Order))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void OrderCompleted_ClickContinue_ExpectedResult()
        {
            const int orderId = 90010;
            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() },
            };

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                Parameters2(orderId));

            CommonActions.ElementIsDisplayed(OrderSummaryObjects.CompleteOrderButton).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.ContinueButton).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DashboardController),
                    nameof(DashboardController.Organisation))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void OrderCompleted_ClickDownloadPdfButton_ExpectedResult()
        {
            const int orderId = 90010;

            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                Parameters2(orderId));

            string filePath = @$"{Path.GetTempPath()}order-summary-completed-C0{orderId}-01.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            CommonActions.ClickLinkElement(OrderSummaryObjects.DownloadPdfButton);

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var order = context.Orders.First(x => x.Id == OrderId);
            order.Completed = null;

            context.SaveChanges();
        }

        private static Dictionary<string, string> Parameters2(int orderId) => new()
        {
            { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), $"{new CallOffId(orderId, 1)}" },
        };

        private static OrderItemFunding CreateOrderItemFunding(OrderItem orderItem)
            => new()
            {
                OrderItem = orderItem,
                CatalogueItemId = orderItem.CatalogueItemId,
                OrderItemFundingType = OrderItemFundingType.CentralFunding,
            };

        private Order CreateOrder(
            bool isAssociatedServiceOnly,
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
                AssociatedServicesOnly = isAssociatedServiceOnly,
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
                    FirstName = "Bruce", LastName = "Wayne", Email = "bat.man@Gotham.Fake", Phone = "123456789",
                },
                CommencementDate = timeNow.AddDays(1),
                InitialPeriod = 6,
                MaximumTerm = 36,
                ContractFlags = new ContractFlags
                {
                    UseDefaultImplementationPlan = false, UseDefaultDataProcessing = true,
                },
            };

            if (isAssociatedServiceOnly)
            {
                order.Solution = context.CatalogueItems.First(x => x.CatalogueItemType == CatalogueItemType.Solution);
            }

            var user = GetBuyerUser(context, order.OrderingPartyId);
            var recipients = context.ServiceRecipients.ToList();

            recipients.ForEach(
                r =>
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
                .FirstOrDefault(
                    cp =>
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
