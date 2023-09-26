using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Csv
{
    public static class CsvServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CsvService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task One_OrderItem_One_Recipient_Results_In_One_Row(
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            var recipient = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });

            order.OrderItems = new HashSet<OrderItem>() { orderItem };
            order.OrderRecipients = new HashSet<OrderRecipient>() { recipient };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows(fullOrderStream);

            records.Count().Should().Be(1);
            var record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientId.Should().Be(recipient.OdsCode);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task One_OrderItem_Two_Recipients_Results_In_Two_Rows_One_For_Each_Recipient(
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                CataloguePriceQuantityCalculationType.PerServiceRecipient);

            var recipient1 = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            var recipient2 = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });

            order.OrderItems = new HashSet<OrderItem>() { orderItem };
            order.OrderRecipients = new HashSet<OrderRecipient>()
            {
                recipient1,
                recipient2,
            };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows(fullOrderStream);

            records.Count().Should().Be(2);
            records.Where(r => r.ProductId == originalCatalogueItem.Id.ToString()
                && r.ServiceRecipientId == recipient1.OdsCode).Count().Should().Be(1);
            records.Where(r => r.ProductId == originalCatalogueItem.Id.ToString()
                && r.ServiceRecipientId == recipient2.OdsCode).Count().Should().Be(1);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Amendendment_One_Recipient_Added_To_One_OrderItem_One_Recipient_Results_In_One_Row(
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
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
            await service.CreateFullOrderCsvAsync(amend.Id, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows(fullOrderStream);

            records.Count().Should().Be(1);
            var record = records.First();
            record.ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            record.ServiceRecipientId.Should().Be(addedRecipient.OdsCode);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Amendendment_One_Recipient_And_One_Service_Added_To_One_OrderItem_One_Recipient(
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            CatalogueItem addedCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
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
            await service.CreateFullOrderCsvAsync(amend.Id, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows(fullOrderStream);

            records.Count().Should().Be(3);
            records.Where(r => r.ServiceRecipientId == addedRecipient.OdsCode && r.ProductId == addedCatalogueItem.Id.ToString())
                .FirstOrDefault()
                .Should().NotBeNull();
            records.Where(r => r.ServiceRecipientId == addedRecipient.OdsCode && r.ProductId == originalCatalogueItem.Id.ToString())
                .FirstOrDefault()
                .Should().NotBeNull();
            records.Where(r => r.ServiceRecipientId == originalRecipient.OdsCode && r.ProductId == addedCatalogueItem.Id.ToString())
                .FirstOrDefault()
                .Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task One_OrderItem_Two_Recipients_With_PerOrderItemQuantity_Results_In_One_Row(
            Order order,
            CsvService service,
            CatalogueItem originalCatalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(
                fixture,
                originalCatalogueItem,
                OrderItemFundingType.LocalFunding,
                CataloguePriceQuantityCalculationType.PerSolutionOrService);

            var recipient1 = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });
            var recipient2 = BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id });

            order.OrderItems = new HashSet<OrderItem>() { orderItem };
            order.OrderRecipients = new HashSet<OrderRecipient>()
        {
            recipient1,
            recipient2,
        };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await using var fullOrderStream = new MemoryStream();
            await service.CreateFullOrderCsvAsync(order.Id, fullOrderStream);
            fullOrderStream.Position = 0;

            var records = GetRows(fullOrderStream);

            records.Count().Should().Be(1);
            records.First().ProductId.Should().Be(originalCatalogueItem.Id.ToString());
            records.First().ServiceRecipientId.Should().Be(order.OrderingParty.ExternalIdentifier);
        }

        private static IEnumerable<FullOrderCsvModel> GetRows(MemoryStream fullOrderStream)
        {
            IEnumerable<FullOrderCsvModel> records;
            using var streamReader = new StreamReader(fullOrderStream);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));
            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(new TypeConverterOptions { Formats = new[] { "dd/MM/yyyy" } });
            csv.Context.TypeConverterCache.AddConverter<CallOffId>(new CallOffIdConverter());
            csv.Context.RegisterClassMap<FullOrderCsvModelMap>();
            records = csv.GetRecords<FullOrderCsvModel>();
            return records.ToList();
        }

        private static OrderItem BuildOrderItem(
            IFixture fixture,
            CatalogueItem catalogueItem,
            OrderItemFundingType? fundingType,
            CataloguePriceQuantityCalculationType cataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient)
        {
            var itemPrice = fixture.Build<OrderItemPrice>()
                .Without(p => p.OrderItem)
                .With(p => p.OrderItemPriceTiers, new HashSet<OrderItemPriceTier>())
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
