using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Csv
{
    public static class FullOrderCsvModelMapTests
    {
        [Theory]
        [InlineData(0, nameof(FullOrderCsvModel.CallOffId), "Call Off Agreement ID")]
        [InlineData(1, nameof(FullOrderCsvModel.OdsCode), "Call Off Ordering Party ID")]
        [InlineData(2, nameof(FullOrderCsvModel.OrganisationName), "Call Off Ordering Party Name")]
        [InlineData(3, nameof(FullOrderCsvModel.CommencementDate), "Call Off Commencement Date")]
        [InlineData(4, nameof(FullOrderCsvModel.ServiceRecipientId), "Service Recipient ID")]
        [InlineData(5, nameof(FullOrderCsvModel.ServiceRecipientName), "Service Recipient Name")]
        [InlineData(6, nameof(FullOrderCsvModel.ServiceRecipientItemId), "Service Recipient Item ID")]
        [InlineData(7, nameof(FullOrderCsvModel.SupplierId), "Supplier ID")]
        [InlineData(8, nameof(FullOrderCsvModel.SupplierName), "Supplier Name")]
        [InlineData(9, nameof(FullOrderCsvModel.ProductId), "Product ID")]
        [InlineData(10, nameof(FullOrderCsvModel.ProductName), "Product Name")]
        [InlineData(11, nameof(FullOrderCsvModel.ProductType), "Product Type")]
        [InlineData(12, nameof(FullOrderCsvModel.QuantityOrdered), "Quantity Ordered")]
        [InlineData(13, nameof(FullOrderCsvModel.UnitOfOrder), "Unit of Order")]
        [InlineData(14, nameof(FullOrderCsvModel.UnitTime), "Unit Time")]
        [InlineData(15, nameof(FullOrderCsvModel.EstimationPeriod), "Estimation Period")]
        [InlineData(16, nameof(FullOrderCsvModel.Price), "Price")]
        [InlineData(17, nameof(FullOrderCsvModel.OrderType), "Order Type")]
        [InlineData(18, nameof(FullOrderCsvModel.FundingType), "Funding Type")]
        [InlineData(19, nameof(FullOrderCsvModel.M1Planned), "M1 planned (Delivery Date)")]
        [InlineData(20, nameof(FullOrderCsvModel.ActualM1Date), "Actual M1 date")]
        [InlineData(21, nameof(FullOrderCsvModel.VerficationDate), "Buyer verification date (M2)")]
        [InlineData(22, nameof(FullOrderCsvModel.CeaseDate), "Cease Date")]
        [InlineData(23, nameof(FullOrderCsvModel.Framework), "Framework")]
        [InlineData(24, nameof(FullOrderCsvModel.InitialTerm), "Initial Term")]
        [InlineData(25, nameof(FullOrderCsvModel.MaximumTerm), "Contract Length (Months)")]
        [InlineData(26, nameof(FullOrderCsvModel.PricingType), "Pricing Type")]
        [InlineData(27, nameof(FullOrderCsvModel.TieredArray), "Tiered Array")]

        public static void ModelMap_ShouldBe_CorrectlyMapped(int index, string memberName, string name)
        {
            var map = new FullOrderCsvModelMap();

            map.MemberMaps.Count.Should().Be(28);
            map.MemberMaps[index].Data.Member.Name.Should().Be(memberName);
            map.MemberMaps[index].Data.Names[0].Should().Be(name);
        }
    }
}
