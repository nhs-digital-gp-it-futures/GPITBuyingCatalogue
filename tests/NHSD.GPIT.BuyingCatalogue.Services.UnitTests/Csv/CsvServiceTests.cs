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
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CsvService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(0, "Call Off Agreement ID")]
        [MockInMemoryDbInlineAutoData(1, "Call Off Ordering Party ID")]
        [MockInMemoryDbInlineAutoData(2, "Call Off Ordering Party Name")]
        [MockInMemoryDbInlineAutoData(3, "Call Off Commencement Date")]
        [MockInMemoryDbInlineAutoData(4, "Service Recipient ID")]
        [MockInMemoryDbInlineAutoData(5, "Service Recipient Name")]
        [MockInMemoryDbInlineAutoData(6, "Service Recipient Item ID")]
        [MockInMemoryDbInlineAutoData(7, "Supplier ID")]
        [MockInMemoryDbInlineAutoData(8, "Supplier Name")]
        [MockInMemoryDbInlineAutoData(9, "Product ID")]
        [MockInMemoryDbInlineAutoData(10, "Product Name")]
        [MockInMemoryDbInlineAutoData(11, "Product Type")]
        [MockInMemoryDbInlineAutoData(12, "Quantity Ordered")]
        [MockInMemoryDbInlineAutoData(13, "Unit of Order")]
        [MockInMemoryDbInlineAutoData(14, "Unit Time")]
        [MockInMemoryDbInlineAutoData(15, "Estimation Period")]
        [MockInMemoryDbInlineAutoData(16, "Price")]
        [MockInMemoryDbInlineAutoData(17, "Order Type")]
        [MockInMemoryDbInlineAutoData(18, "Funding Type")]
        [MockInMemoryDbInlineAutoData(19, "M1 planned (Delivery Date)")]
        [MockInMemoryDbInlineAutoData(20, "Actual M1 date")]
        [MockInMemoryDbInlineAutoData(21, "Buyer verification date (M2)")]
        [MockInMemoryDbInlineAutoData(22, "Cease Date")]
        [MockInMemoryDbInlineAutoData(23, "Framework")]
        [MockInMemoryDbInlineAutoData(24, "Initial Term")]
        [MockInMemoryDbInlineAutoData(25, "Contract Length (Months)")]
        [MockInMemoryDbInlineAutoData(26, "Pricing Type")]
        [MockInMemoryDbInlineAutoData(27, "Tiered Array")]
        public static async Task SolutionOrder_Mapped_Columns(
            int index,
            string name,
            Order order,
            CsvService service,
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            order.OrderType = OrderTypeEnum.Solution;
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
            csv.GetField(index).Should().Be(name);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(0, "Call Off Agreement ID")]
        [MockInMemoryDbInlineAutoData(1, "Call Off Ordering Party ID")]
        [MockInMemoryDbInlineAutoData(2, "Call Off Ordering Party Name")]
        [MockInMemoryDbInlineAutoData(3, "Call Off Commencement Date")]
        [MockInMemoryDbInlineAutoData(4, "Service Recipient ID")]
        [MockInMemoryDbInlineAutoData(5, "Service Recipient Name")]
        [MockInMemoryDbInlineAutoData(6, "Service Recipient Item ID")]
        [MockInMemoryDbInlineAutoData(7, "Supplier ID")]
        [MockInMemoryDbInlineAutoData(8, "Supplier Name")]
        [MockInMemoryDbInlineAutoData(9, "Product ID")]
        [MockInMemoryDbInlineAutoData(10, "Product Name")]
        [MockInMemoryDbInlineAutoData(11, "Product Type")]
        [MockInMemoryDbInlineAutoData(12, "Quantity Ordered")]
        [MockInMemoryDbInlineAutoData(13, "Unit of Order")]
        [MockInMemoryDbInlineAutoData(14, "Unit Time")]
        [MockInMemoryDbInlineAutoData(15, "Estimation Period")]
        [MockInMemoryDbInlineAutoData(16, "Price")]
        [MockInMemoryDbInlineAutoData(17, "Order Type")]
        [MockInMemoryDbInlineAutoData(18, "Funding Type")]
        [MockInMemoryDbInlineAutoData(19, "M1 planned (Delivery Date)")]
        [MockInMemoryDbInlineAutoData(20, "Actual M1 date")]
        [MockInMemoryDbInlineAutoData(21, "Buyer verification date (M2)")]
        [MockInMemoryDbInlineAutoData(22, "Cease Date")]
        [MockInMemoryDbInlineAutoData(23, "Framework")]
        [MockInMemoryDbInlineAutoData(24, "Initial Term")]
        [MockInMemoryDbInlineAutoData(25, "Contract Length (Months)")]
        [MockInMemoryDbInlineAutoData(26, "Pricing Type")]
        [MockInMemoryDbInlineAutoData(27, "Tiered Array")]
        public static async Task AssociatedServiceOtherOrder_Mapped_Columns(
            int index,
            string name,
            Order order,
            CsvService service,
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
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
            csv.GetField(index).Should().Be(name);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(0, "Call Off Agreement ID")]
        [MockInMemoryDbInlineAutoData(1, "Call Off Ordering Party ID")]
        [MockInMemoryDbInlineAutoData(2, "Call Off Ordering Party Name")]
        [MockInMemoryDbInlineAutoData(3, "Call Off Commencement Date")]
        [MockInMemoryDbInlineAutoData(4, "Practice to close / become branch site (ODS code)")]
        [MockInMemoryDbInlineAutoData(5, "Practice to be retained (ODS code)")]
        [MockInMemoryDbInlineAutoData(6, "Service Recipient Item ID")]
        [MockInMemoryDbInlineAutoData(7, "Supplier ID")]
        [MockInMemoryDbInlineAutoData(8, "Supplier Name")]
        [MockInMemoryDbInlineAutoData(9, "Product ID")]
        [MockInMemoryDbInlineAutoData(10, "Product Name")]
        [MockInMemoryDbInlineAutoData(11, "Product Type")]
        [MockInMemoryDbInlineAutoData(12, "Quantity Ordered")]
        [MockInMemoryDbInlineAutoData(13, "Unit of Order")]
        [MockInMemoryDbInlineAutoData(14, "Unit Time")]
        [MockInMemoryDbInlineAutoData(15, "Estimation Period")]
        [MockInMemoryDbInlineAutoData(16, "Price")]
        [MockInMemoryDbInlineAutoData(17, "Order Type")]
        [MockInMemoryDbInlineAutoData(18, "Funding Type")]
        [MockInMemoryDbInlineAutoData(19, "M1 planned (Delivery Date)")]
        [MockInMemoryDbInlineAutoData(20, "Actual M1 date")]
        [MockInMemoryDbInlineAutoData(21, "Buyer verification date (M2)")]
        [MockInMemoryDbInlineAutoData(22, "Cease Date")]
        [MockInMemoryDbInlineAutoData(23, "Framework")]
        [MockInMemoryDbInlineAutoData(24, "Initial Term")]
        [MockInMemoryDbInlineAutoData(25, "Contract Length (Months)")]
        [MockInMemoryDbInlineAutoData(26, "Pricing Type")]
        [MockInMemoryDbInlineAutoData(27, "Tiered Array")]
        public static async Task MergerOrder_Mapped_Columns(
            int index,
            string name,
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
            csv.GetField(index).Should().Be(name);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(0, "Call Off Agreement ID")]
        [MockInMemoryDbInlineAutoData(1, "Call Off Ordering Party ID")]
        [MockInMemoryDbInlineAutoData(2, "Call Off Ordering Party Name")]
        [MockInMemoryDbInlineAutoData(3, "Call Off Commencement Date")]
        [MockInMemoryDbInlineAutoData(4, "Practice to split (ODS code)")]
        [MockInMemoryDbInlineAutoData(5, "Practice to be retained (ODS code)")]
        [MockInMemoryDbInlineAutoData(6, "Service Recipient Item ID")]
        [MockInMemoryDbInlineAutoData(7, "Supplier ID")]
        [MockInMemoryDbInlineAutoData(8, "Supplier Name")]
        [MockInMemoryDbInlineAutoData(9, "Product ID")]
        [MockInMemoryDbInlineAutoData(10, "Product Name")]
        [MockInMemoryDbInlineAutoData(11, "Product Type")]
        [MockInMemoryDbInlineAutoData(12, "Quantity Ordered")]
        [MockInMemoryDbInlineAutoData(13, "Unit of Order")]
        [MockInMemoryDbInlineAutoData(14, "Unit Time")]
        [MockInMemoryDbInlineAutoData(15, "Estimation Period")]
        [MockInMemoryDbInlineAutoData(16, "Price")]
        [MockInMemoryDbInlineAutoData(17, "Order Type")]
        [MockInMemoryDbInlineAutoData(18, "Funding Type")]
        [MockInMemoryDbInlineAutoData(19, "M1 planned (Delivery Date)")]
        [MockInMemoryDbInlineAutoData(20, "Actual M1 date")]
        [MockInMemoryDbInlineAutoData(21, "Buyer verification date (M2)")]
        [MockInMemoryDbInlineAutoData(22, "Cease Date")]
        [MockInMemoryDbInlineAutoData(23, "Framework")]
        [MockInMemoryDbInlineAutoData(24, "Initial Term")]
        [MockInMemoryDbInlineAutoData(25, "Contract Length (Months)")]
        [MockInMemoryDbInlineAutoData(26, "Pricing Type")]
        [MockInMemoryDbInlineAutoData(27, "Tiered Array")]
        public static async Task SplitOrder_Mapped_Columns(
            int index,
            string name,
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
            csv.GetField(index).Should().Be(name);
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

            var recipient = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            await SaveOrderWithRecipients(
                order,
                originalCatalogueItem,
                CataloguePriceQuantityCalculationType.PerServiceRecipient,
                provisioningType,
                new HashSet<OrderRecipient>() { recipient },
                dbContext,
                fixture);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap());

            records.Count.Should().Be(1);
            var record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientId.Should().Be(recipient.OdsCode);
            record.ServiceRecipientName.Should().Be(recipient.OdsOrganisation.Name);
            record.ServiceRecipientItemId.Should().StartWith($"{order.CallOffId}-{recipient.OdsCode}-0");
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
            order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Id = order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode;

            var recipient = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            await SaveOrderWithRecipients(
                order,
                originalCatalogueItem,
                CataloguePriceQuantityCalculationType.PerServiceRecipient,
                provisioningType,
                new HashSet<OrderRecipient>() { recipient },
                dbContext,
                fixture);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows<MergerOrderCsvModel>(fullOrderStream, new MergerOrderCsvModelMap(FullOrderCsvModelMap.Names));

            records.Count.Should().Be(1);
            var record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientToClose.Should()
                .Be($"{recipient.OdsOrganisation.Name} ({recipient.OdsCode})");
            record.ServiceRecipientToRetain.Should()
                .Be($"{order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name} ({order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Id})");
            record.ServiceRecipientItemId.Should()
                .StartWith($"{order.CallOffId}-{recipient.OdsCode}-0");
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

            var recipient = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            await SaveOrderWithRecipients(
                order,
                originalCatalogueItem,
                CataloguePriceQuantityCalculationType.PerServiceRecipient,
                provisioningType,
                new HashSet<OrderRecipient>() { recipient },
                dbContext,
                fixture);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows<SplitOrderCsvModel>(fullOrderStream, new SplitOrderCsvModelMap(FullOrderCsvModelMap.Names));

            records.Count.Should().Be(1);
            var record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientToRetain.Should()
                .Be($"{recipient.OdsOrganisation.Name} ({recipient.OdsCode})");
            record.ServiceRecipientToSplit.Should()
                .Be($"{order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name} ({order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Id})");
            record.ServiceRecipientItemId.Should()
                .StartWith($"{order.CallOffId}-{order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Id}-0");
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

            var recipient1 = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            var recipient2 = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            await SaveOrderWithRecipients(
                order,
                originalCatalogueItem,
                CataloguePriceQuantityCalculationType.PerServiceRecipient,
                provisioningType,
                new HashSet<OrderRecipient>() { recipient1, recipient2 },
                dbContext,
                fixture);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap()).ToList();

            records.Count.Should().Be(2);
            records[0].ServiceRecipientItemId.Should().EndWith("-0");
            records[1].ServiceRecipientItemId.Should().EndWith("-1");

            var record1 = records
                .FirstOrDefault(r => r.ServiceRecipientId == recipient1.OdsCode);
            record1.Should().NotBeNull();
            record1!.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record1.ServiceRecipientName.Should().Be(recipient1.OdsOrganisation.Name);
            record1.ServiceRecipientItemId.Should().StartWith($"{order.CallOffId}-{recipient1.OdsCode}-");

            var record2 = records
                .FirstOrDefault(r => r.ServiceRecipientId == recipient2.OdsCode);
            record2.Should().NotBeNull();
            record2!.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record2.ServiceRecipientName.Should().Be(recipient2.OdsOrganisation.Name);
            record2.ServiceRecipientItemId.Should().StartWith($"{order.CallOffId}-{recipient2.OdsCode}-");
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

            var recipient = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });

            order.Revision = 1;
            order.OrderingPartyId = order.OrderingParty.Id;

            order.OrderItems = new HashSet<OrderItem>() { orderItem };
            order.OrderRecipients = new HashSet<OrderRecipient>() { recipient };

            var amend = order.BuildAmendment(2);
            var addedRecipient = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            amend.OrderRecipients.Add(addedRecipient);
            amend.OrderItems.First().OrderItemFunding = BuildFunding(fixture, OrderItemFundingType.NoFundingRequired);

            dbContext.Orders.Add(order);
            dbContext.Orders.Add(amend);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(amend.Id, amend.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap());

            records.Count().Should().Be(1);
            var record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientId.Should().Be(addedRecipient.OdsCode);
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

            var recipient = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });

            order.Revision = 1;
            order.OrderingPartyId = order.OrderingParty.Id;

            order.OrderItems = new HashSet<OrderItem>() { orderItem };
            order.OrderRecipients = new HashSet<OrderRecipient>() { recipient };

            var amend = order.BuildAmendment(2);
            OrderItem addedOrderItem = BuildOrderItem(
                fixture,
                addedCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);
            var originalRecipient = amend.OrderRecipients.First();
            originalRecipient.SetQuantityForItem(addedCatalogueItem.Id, 1);
            var addedRecipient = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id, addedCatalogueItem.Id });
            amend.OrderRecipients.Add(addedRecipient);
            amend.OrderItems.Add(addedOrderItem);
            amend.OrderItems.First().OrderItemFunding = BuildFunding(fixture, OrderItemFundingType.NoFundingRequired);

            dbContext.Orders.Add(order);
            dbContext.Orders.Add(amend);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(amend.Id, amend.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap()).ToList();

            records.Count.Should().Be(3);
            records[0].ServiceRecipientItemId.Should().EndWith("-0");
            records[1].ServiceRecipientItemId.Should().EndWith("-1");
            records[2].ServiceRecipientItemId.Should().EndWith("-2");

            records
                .FirstOrDefault(r => r.ServiceRecipientId == addedRecipient.OdsCode && r.ProductId == addedCatalogueItem.Id.ToString())
                .Should().NotBeNull();
            records
                .FirstOrDefault(r => r.ServiceRecipientId == addedRecipient.OdsCode && r.ProductId == originalCatalogueItem.Id.ToString())
                .Should().NotBeNull();
            records
                .FirstOrDefault(r => r.ServiceRecipientId == originalRecipient.OdsCode && r.ProductId == addedCatalogueItem.Id.ToString())
                .Should().NotBeNull();
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

            var recipient1 = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            var recipient2 = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            await SaveOrderWithRecipients(
                order,
                originalCatalogueItem,
                CataloguePriceQuantityCalculationType.PerSolutionOrService,
                provisioningType,
                new HashSet<OrderRecipient>() { recipient1, recipient2 },
                dbContext,
                fixture);

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows<FullOrderCsvModel>(fullOrderStream, new FullOrderCsvModelMap()).ToList();

            records.Count.Should().Be(1);
            records[0].ServiceRecipientItemId.Should().EndWith("-0");
            records.First().ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            records.First().ServiceRecipientId.Should().Be(order.OrderingParty.ExternalIdentifier);
            records.First().ServiceRecipientName.Should().Be(order.OrderingParty.Name);
            records.First().ServiceRecipientItemId.Should().StartWith($"{order.CallOffId}-{order.OrderingParty.ExternalIdentifier}-");
        }

        private static async Task SaveOrderWithRecipients(Order order, CatalogueItem originalCatalogueItem, CataloguePriceQuantityCalculationType cataloguePriceQuantityCalculationType, ProvisioningType provisioningType, ICollection<OrderRecipient> recipients, BuyingCatalogueDbContext dbContext, IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                provisioningType,
                cataloguePriceQuantityCalculationType);

            order.OrderItems = new HashSet<OrderItem>() { orderItem };
            order.OrderRecipients = recipients;

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();
        }

        private static List<T> GetRows<T>(Stream fullOrderStream, ClassMap map)
        {
            using var streamReader = new StreamReader(fullOrderStream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));
            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(new TypeConverterOptions { Formats = new[] { "dd/MM/yyyy" } });
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

        private static OrderRecipient BuildOrderRecipient(IFixture fixture, CatalogueItemId[] catalogueItemIds = null)
        {
            var recipient = fixture.Build<OrderRecipient>()
                .Without(r => r.OrderItemRecipients)
                .Create();
            recipient.OdsCode = recipient.OdsOrganisation.Id;

            UpdateRecipientToItem(recipient, catalogueItemIds);

            return recipient;
        }

        private static void UpdateRecipientToItem(OrderRecipient recipient, CatalogueItemId[] catalogueItemIds)
        {
            if (catalogueItemIds != null)
            {
                foreach (var catalogueItemId in catalogueItemIds)
                {
                    recipient.SetQuantityForItem(catalogueItemId, 1);
                }
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
