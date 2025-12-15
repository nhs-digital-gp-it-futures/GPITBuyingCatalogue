using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Csv
{
    public static class CsvServiceTests
    {
        private static IEnumerable<string> OrderFields =>
        [
            "Call Off Agreement ID",
            "Call Off Ordering Party ID",
            "Call Off Ordering Party Name",
            "Sub ICB Code",
            "Sub ICB Name",
            "Call Off Commencement Date",
            "Service Recipient ID",
            "Service Recipient Name",
            "Service Recipient Item ID",
            "Supplier ID",
            "Supplier Name",
            "Product ID",
            "Product Name",
            "Product Type",
            "Quantity Ordered",
            "Unit of Order",
            "Unit Time",
            "Estimation Period",
            "Price",
            "Order Type",
            "Funding Type",
            "M1 planned (Delivery Date)",
            "Actual M1 date",
            "Buyer verification date (M2)",
            "Cease Date",
            "Framework",
            "Pricing Type",
            "Tiered Array",
            "Initial Term",
            "Contract Length (Months)",
            "Bespoke Milestones",
        ];

        private static IEnumerable<string> MergerFields =>
        [
            "Call Off Agreement ID",
            "Call Off Ordering Party ID",
            "Call Off Ordering Party Name",
            "Sub ICB Code",
            "Sub ICB Name",
            "Call Off Commencement Date",
            "Practice to close / become branch site (ODS code)",
            "Practice to be retained (ODS code)",
            "Service Recipient Item ID",
            "Supplier ID",
            "Supplier Name",
            "Product ID",
            "Product Name",
            "Product Type",
            "Quantity Ordered",
            "Unit of Order",
            "Unit Time",
            "Estimation Period",
            "Price",
            "Order Type",
            "Funding Type",
            "M1 planned (Delivery Date)",
            "Actual M1 date",
            "Buyer verification date (M2)",
            "Cease Date",
            "Framework",
            "Pricing Type",
            "Tiered Array",
            "Initial Term",
            "Contract Length (Months)",
        ];

        private static IEnumerable<string> SplitFields =>
        [
            "Call Off Agreement ID",
            "Call Off Ordering Party ID",
            "Call Off Ordering Party Name",
            "Sub ICB Code",
            "Sub ICB Name",
            "Call Off Commencement Date",
            "Practice to split (ODS code)",
            "Practice to be retained (ODS code)",
            "Service Recipient Item ID",
            "Supplier ID",
            "Supplier Name",
            "Product ID",
            "Product Name",
            "Product Type",
            "Quantity Ordered",
            "Unit of Order",
            "Unit Time",
            "Estimation Period",
            "Price",
            "Order Type",
            "Funding Type",
            "M1 planned (Delivery Date)",
            "Actual M1 date",
            "Buyer verification date (M2)",
            "Cease Date",
            "Framework",
            "Pricing Type",
            "Tiered Array",
            "Initial Term",
            "Contract Length (Months)",
        ];

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CsvService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.Solution)]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static async Task SolutionOrder_Mapped_Columns(
            OrderTypeEnum orderType,
            Order order,
            CsvService service,
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            order.OrderType = orderType;
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            using var streamReader = new StreamReader(fullOrderStream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
            });

            await csv.ReadAsync();
            csv.ReadHeader();
            OrderFields.Select((x, i) => (Value: x, Index: i))
                .ToList()
                .ForEach(x => csv.GetField(x.Index).Should().Be(x.Value));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task MergerOrder_Mapped_Columns(
            Order order,
            CsvService service,
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceMerger;
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            using var streamReader = new StreamReader(fullOrderStream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
            });

            await csv.ReadAsync();
            csv.ReadHeader();
            MergerFields.Select((x, i) => (Value: x, Index: i))
                .ToList()
                .ForEach(x => csv.GetField(x.Index).Should().Be(x.Value));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SplitOrder_Mapped_Columns(
            Order order,
            CsvService service,
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceSplit;
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            using var streamReader = new StreamReader(fullOrderStream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
            });

            await csv.ReadAsync();
            csv.ReadHeader();
            SplitFields.Select((x, i) => (Value: x, Index: i))
                .ToList()
                .ForEach(x => csv.GetField(x.Index).Should().Be(x.Value));
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task OrderTypeSolution_One_OrderItem_One_Recipient_Results_In_One_Row(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            order.OrderType = OrderTypeEnum.Solution;

            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient = BuildOrderRecipient(fixture, [orderItem]);
            await SaveOrderWithRecipients(
                order,
                [orderItem],
                [recipient],
                dbContext);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<FullOrderCsvModel> records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap());

            records.Count.Should().Be(1);
            FullOrderCsvModel record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientId.Should().Be(recipient.RecipientOdsCode);
            record.ServiceRecipientName.Should().Be(recipient.RecipientOdsOrganisation.Name);
            record.ServiceRecipientItemId.Should().Be($"{order.CallOffId}-{recipient.RecipientOdsCode}-{orderItem.CatalogueItemId}");
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task CreateFullOrderCsv_WithImplementationPlan_SetsBespokeMilestones(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            Solution solution,
            AssociatedService associatedService,
            ImplementationPlan implementationPlan,
            List<ImplementationPlanMilestone> implementationPlanMilestones,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            implementationPlan.Milestones = implementationPlanMilestones;

            order.OrderType = OrderTypeEnum.Solution;
            order.Contract = new() { ImplementationPlan = implementationPlan, };

            var solutionOrderItem = BuildOrderItem(
                fixture,
                solution.CatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            var associatedServiceOrderItem = BuildOrderItem(
                fixture,
                associatedService.CatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient = BuildOrderRecipient(
                fixture,
                [solutionOrderItem, associatedServiceOrderItem]);
            await SaveOrderWithRecipients(
                order,
                [solutionOrderItem, associatedServiceOrderItem],
                [recipient],
                dbContext);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<FullOrderCsvModel> records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap());

            records.Count.Should().Be(2);

            var solutionRecord = records.First(x => x.ProductId == solutionOrderItem.CatalogueItemId.ToString());
            var associatedServiceRecord =
                records.First(x => x.ProductId == associatedServiceOrderItem.CatalogueItemId.ToString());

            solutionRecord.HasBespokeMilestones.Should().BeTrue();
            associatedServiceRecord.HasBespokeMilestones.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task CreateFullOrderCsv_WithAssociatedServiceMilestones_SetsBespokeMilestones(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            Solution solution,
            AssociatedService associatedService,
            ContractBilling contractBilling,
            List<ContractBillingItem> contractBillingItems,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            contractBillingItems.ForEach(x => x.CatalogueItemId = associatedService.CatalogueItemId);
            contractBilling.ContractBillingItems = contractBillingItems;

            order.OrderType = OrderTypeEnum.Solution;
            order.Contract = new() { ContractBilling = contractBilling, };

            var solutionOrderItem = BuildOrderItem(
                fixture,
                solution.CatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            var associatedServiceOrderItem = BuildOrderItem(
                fixture,
                associatedService.CatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient = BuildOrderRecipient(
                fixture,
                [solutionOrderItem, associatedServiceOrderItem]);
            await SaveOrderWithRecipients(
                order,
                [solutionOrderItem, associatedServiceOrderItem],
                [recipient],
                dbContext);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<FullOrderCsvModel> records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap());

            records.Count.Should().Be(2);

            var solutionRecord = records.First(x => x.ProductId == solutionOrderItem.CatalogueItemId.ToString());
            var associatedServiceRecord =
                records.First(x => x.ProductId == associatedServiceOrderItem.CatalogueItemId.ToString());

            solutionRecord.HasBespokeMilestones.Should().BeFalse();
            associatedServiceRecord.HasBespokeMilestones.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task OrderTypeMerger_One_OrderItem_One_Recipient_Results_In_One_Row(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceMerger;
            order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Id =
                order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode;

            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient = BuildOrderRecipient(fixture, [orderItem]);
            await SaveOrderWithRecipients(
                order,
                [orderItem],
                [recipient],
                dbContext);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<MergerOrderCsvModel> records = GetRows<MergerOrderCsvModel>(
                fullOrderStream,
                new MergerOrderCsvModelMap(FullOrderCsvModelMap.Names));

            records.Count.Should().Be(1);
            MergerOrderCsvModel record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientToClose.Should()
                .Be($"{recipient.RecipientOdsOrganisation.Name} ({recipient.RecipientOdsCode})");
            record.ServiceRecipientToRetain.Should()
                .Be(
                    $"{order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name} ({order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Id})");
            record.ServiceRecipientItemId.Should().Be($"{order.CallOffId}-{recipient.RecipientOdsCode}-{orderItem.CatalogueItemId}");
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task OrderTypeSplit_One_OrderItem_One_Recipient_Results_In_One_Row(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceSplit;

            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient = BuildOrderRecipient(fixture, [orderItem]);
            await SaveOrderWithRecipients(
                order,
                [orderItem],
                [recipient],
                dbContext);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<SplitOrderCsvModel> records = GetRows<SplitOrderCsvModel>(
                fullOrderStream,
                new SplitOrderCsvModelMap(FullOrderCsvModelMap.Names));

            records.Count.Should().Be(1);
            SplitOrderCsvModel record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientToRetain.Should()
                .Be($"{recipient.RecipientOdsOrganisation.Name} ({recipient.RecipientOdsCode})");
            record.ServiceRecipientToSplit.Should()
                .Be(
                    $"{order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name} ({order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Id})");
            record.ServiceRecipientItemId.Should().Be($"{order.CallOffId}-{recipient.RecipientOdsCode}-{orderItem.CatalogueItemId}");
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task OrderTypeSolution_One_OrderItem_Two_Recipients_Results_In_Two_Rows_One_For_Each_Recipient(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            order.OrderType = OrderTypeEnum.Solution;

            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient1 = BuildOrderRecipient(fixture, [orderItem]);
            OrderSublocationRecipient recipient2 = BuildOrderRecipient(fixture, [orderItem]);
            await SaveOrderWithRecipients(
                order,
                [orderItem],
                [recipient1, recipient2],
                dbContext);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<FullOrderCsvModel> records =
                GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap()).ToList();

            records.Count.Should().Be(2);

            FullOrderCsvModel record1 = records
                .FirstOrDefault(r => r.ServiceRecipientId == recipient1.RecipientOdsCode);
            record1.Should().NotBeNull();
            record1!.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record1.ServiceRecipientName.Should().Be(recipient1.RecipientOdsOrganisation.Name);
            record1.ServiceRecipientItemId.Should().Be($"{order.CallOffId}-{recipient1.RecipientOdsCode}-{orderItem.CatalogueItemId}");

            FullOrderCsvModel record2 = records
                .FirstOrDefault(r => r.ServiceRecipientId == recipient2.RecipientOdsCode);
            record2.Should().NotBeNull();
            record2!.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record2.ServiceRecipientName.Should().Be(recipient2.RecipientOdsOrganisation.Name);
            record2.ServiceRecipientItemId.Should().Be($"{order.CallOffId}-{recipient2.RecipientOdsCode}-{orderItem.CatalogueItemId}");
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        public static async Task One_OrderItem_Two_Recipients_With_PerOrderItemQuantity_Results_In_One_Row(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            order.OrderType = OrderTypeEnum.Solution;

            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerSolutionOrService);

            OrderSublocationRecipient recipient1 = BuildOrderRecipient(fixture, [orderItem]);
            OrderSublocationRecipient recipient2 = BuildOrderRecipient(fixture, [orderItem]);
            await SaveOrderWithRecipients(
                order,
                [orderItem],
                [recipient1, recipient2],
                dbContext);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<FullOrderCsvModel> records =
                GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap()).ToList();

            records.Count.Should().Be(1);
            records.First().ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            records.First().ServiceRecipientId.Should().Be(order.OrderingParty.ExternalIdentifier);
            records.First().ServiceRecipientName.Should().Be(order.OrderingParty.Name);
            records.First().ServiceRecipientItemId.Should().Be($"{order.CallOffId}-{order.OrderingParty.ExternalIdentifier}-{orderItem.CatalogueItemId}");
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task Amendment_One_Recipient_Added_To_One_OrderItem_One_Recipient_Results_In_One_Row(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            order.OrderType = OrderTypeEnum.Solution;
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient = BuildOrderRecipient(fixture, [orderItem]);

            order.OrderNumber = order.ContractOrderNumber.Id;
            order.Revision = 1;
            order.OrderingPartyId = order.OrderingParty.Id;

            order.OrderSublocations = order.OrderSublocations.Take(1).ToList();

            OrderSublocation workingSublocation = order.OrderSublocations.First();

            workingSublocation.Order = order;

            order.OrderItems = new HashSet<OrderItem> { orderItem };

            workingSublocation.SublocationRecipients = [recipient];

            Order amend = order.BuildAmendment(2);

            OrderSublocationRecipient addedRecipient = BuildOrderRecipient(fixture, [orderItem]);
            var amendSublocation = amend.OrderSublocations.First();
            amendSublocation.SublocationOrganisation = workingSublocation.SublocationOrganisation;
            amendSublocation.SublocationRecipients.Add(addedRecipient);
            amend.OrderItems.First().OrderItemFunding = BuildFunding(fixture, OrderItemFundingType.NoFundingRequired);

            dbContext.Orders.Add(order);
            dbContext.Orders.Add(amend);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(amend.Id, amend.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<FullOrderCsvModel> records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap());

            records.Count().Should().Be(1);
            FullOrderCsvModel record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientId.Should().Be(addedRecipient.RecipientOdsCode);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task Amendment_One_Recipient_And_One_Service_Added_To_One_OrderItem_One_Recipient(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            CatalogueItem addedCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            order.OrderType = OrderTypeEnum.Solution;
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient = BuildOrderRecipient(fixture, [orderItem]);

            orderItem.Order = order;

            order.OrderNumber = order.ContractOrderNumber.Id;
            order.Revision = 1;
            order.OrderingPartyId = order.OrderingParty.Id;

            order.OrderSublocations = order.OrderSublocations.Take(1).ToList();

            OrderSublocation workingSublocation = order.OrderSublocations.First();

            workingSublocation.Order = order;

            order.OrderItems = new HashSet<OrderItem> { orderItem };

            workingSublocation.SublocationRecipients = [recipient];

            Order amend = order.BuildAmendment(2);

            OrderItem addedOrderItem = BuildOrderItem(
                fixture,
                addedCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);
            OrderSublocationRecipient originalRecipient = amend.FlattenedRecipients.First();
            originalRecipient.SetQuantityForItem(addedCatalogueItem.Id, 1);
            OrderSublocationRecipient addedRecipient = BuildOrderRecipient(
                fixture,
                [orderItem, addedOrderItem]);
            var amendSublocation = amend.OrderSublocations.First();
            amendSublocation.SublocationOrganisation = workingSublocation.SublocationOrganisation;
            amendSublocation.SublocationRecipients.Add(addedRecipient);
            amend.OrderItems.Add(addedOrderItem);
            amend.OrderItems.First().OrderItemFunding = BuildFunding(fixture, OrderItemFundingType.NoFundingRequired);

            dbContext.Orders.Add(order);
            dbContext.Orders.Add(amend);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(amend.Id, amend.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            List<FullOrderCsvModel> records =
                GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap()).ToList();

            records.Count.Should().Be(3);
            records
                .FirstOrDefault(r =>
                    r.ServiceRecipientId == addedRecipient.RecipientOdsCode
                    && r.ProductId == addedCatalogueItem.Id.ToString())
                .Should()
                .NotBeNull();
            records
                .FirstOrDefault(r =>
                    r.ServiceRecipientId == addedRecipient.RecipientOdsCode
                    && r.ProductId == originalCatalogueItem.Id.ToString())
                .Should()
                .NotBeNull();
            records
                .FirstOrDefault(r =>
                    r.ServiceRecipientId == originalRecipient.RecipientOdsCode
                    && r.ProductId == addedCatalogueItem.Id.ToString())
                .Should()
                .NotBeNull();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(ProvisioningType.OnDemand)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Declarative)]
        [MockInMemoryDbInlineAutoData(ProvisioningType.Patient)]
        public static async Task Amendment_ShowRevisions_Results_In_Two_Rows(
            ProvisioningType provisioningType,
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            order.OrderType = OrderTypeEnum.Solution;
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            OrderSublocationRecipient recipient = BuildOrderRecipient(fixture, [orderItem]);

            orderItem.Order = order;

            order.OrderNumber = order.ContractOrderNumber.Id;
            order.Revision = 1;
            order.OrderingPartyId = order.OrderingParty.Id;

            order.OrderSublocations = order.OrderSublocations.Take(1).ToList();

            OrderSublocation workingSublocation = order.OrderSublocations.First();

            workingSublocation.Order = order;

            order.OrderItems = new HashSet<OrderItem> { orderItem };

            workingSublocation.SublocationRecipients = [recipient];

            Order amend = order.BuildAmendment(2);

            OrderSublocationRecipient addedRecipient = BuildOrderRecipient(fixture, [orderItem]);
            var amendSublocation = amend.OrderSublocations.First();
            amendSublocation.SublocationOrganisation = workingSublocation.SublocationOrganisation;
            amendSublocation.SublocationRecipients.Add(addedRecipient);
            amend.OrderItems.First().OrderItemFunding = BuildFunding(fixture, OrderItemFundingType.NoFundingRequired);

            dbContext.Orders.Add(order);
            dbContext.Orders.Add(amend);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(amend.Id, amend.OrderType, fullOrderStream, true);
            fullOrderStream.Position = 0;

            List<FullOrderCsvModel> records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap());

            records.Count.Should().Be(2);
            records.Should().Contain(x => x.CallOffId == order.CallOffId);
            records.Should().Contain(x => x.CallOffId == amend.CallOffId);
        }

        private static async Task SaveOrderWithRecipients(
            Order order,
            ICollection<OrderItem> orderItems,
            ICollection<OrderSublocationRecipient> recipients,
            BuyingCatalogueDbContext dbContext)
        {
            order.OrderItems = orderItems.ToHashSet();

            order.OrderSublocations = order.OrderSublocations.Take(1).ToList();

            OrderSublocation workingSublocation = order.OrderSublocations.First();

            workingSublocation.Order = order;
            workingSublocation.SublocationRecipients = recipients;

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();
        }

        private static List<T> GetRows<T>(Stream fullOrderStream, ClassMap map)
        {
            using var streamReader = new StreamReader(fullOrderStream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));
            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(
                new TypeConverterOptions
                {
                    Formats =
                        ["dd/MM/yyyy"],
                });
            csv.Context.TypeConverterCache.AddConverter<CallOffId>(new CallOffIdConverter());
            csv.Context.RegisterClassMap(map);

            IEnumerable<T> records = csv.GetRecords<T>();
            return records.ToList();
        }

        private static OrderItem BuildOrderItem(
            IFixture fixture,
            CatalogueItem catalogueItem,
            OrderItemFundingType? fundingType,
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType cataloguePriceQuantityCalculationType)
        {
            var itemPrice = fixture.Build<OrderItemPrice>()
                .Without(p => p.OrderItem)
                .With(p => p.OrderItemPriceTiers, new HashSet<OrderItemPriceTier>())
                .With(p => p.ProvisioningType, provisioningType)
                .With(p => p.CataloguePriceQuantityCalculationType, cataloguePriceQuantityCalculationType)
                .Create() as IPrice;

            var funding = fundingType.HasValue
                ? BuildFunding(fixture, fundingType)
                : null;

            var orderItem = fixture.Build<OrderItem>()
                .Without(i => i.Order)
                .With(i => i.CatalogueItem, catalogueItem)
                .With(i => i.CatalogueItemId, catalogueItem.Id)
                .With(i => i.OrderItemPrice, itemPrice)
                .With(i => i.OrderItemFunding, funding)
                .Create();

            return orderItem;
        }

        private static OrderItemFunding BuildFunding(IFixture fixture, OrderItemFundingType? fundingType)
        {
            return fixture.Build<OrderItemFunding>()
                            .Without(p => p.OrderItem)
                            .With(f => f.OrderItemFundingType, fundingType)
                            .Create();
        }

        private static OrderSublocationRecipient BuildOrderRecipient(
            IFixture fixture,
            OrderItem[] orderItems = null)
        {
            OrderSublocationRecipient recipient = fixture.Build<OrderSublocationRecipient>()
                .Without(r => r.OrderItemSublocationRecipients)
                .Create();
            recipient.RecipientOdsCode = recipient.RecipientOdsOrganisation.Id;

            UpdateRecipientToItem(recipient, orderItems);

            return recipient;
        }

        private static void UpdateRecipientToItem(
            OrderSublocationRecipient recipient,
            OrderItem[] orderItems)
        {
            foreach (OrderItem orderItem in orderItems)
            {
                recipient.OrderItemSublocationRecipients.Add(
                    new OrderItemSublocationRecipient
                    {
                        OrderId = recipient.OrderId,
                        RecipientOdsCode = recipient.RecipientOdsCode,
                        CatalogueItemId = orderItem.CatalogueItemId,
                        Quantity = 1,
                        Recipient = recipient,
                        OrderItem = orderItem,
                    });
            }
        }

        public class CallOffIdConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return CallOffId.Parse(text).Id;
            }
        }
    }
}
