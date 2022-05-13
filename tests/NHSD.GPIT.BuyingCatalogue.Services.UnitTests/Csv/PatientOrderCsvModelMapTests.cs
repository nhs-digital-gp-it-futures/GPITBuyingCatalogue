using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Csv
{
    public static class PatientOrderCsvModelMapTests
    {
        [Theory]
        [InlineData(0, nameof(PatientOrderCsvModel.CallOffId), "Call Off Agreement ID")]
        [InlineData(1, nameof(PatientOrderCsvModel.OdsCode), "Call Off Ordering Party ID")]
        [InlineData(2, nameof(PatientOrderCsvModel.OrganisationName), "Call Off Ordering Party Name")]
        [InlineData(3, nameof(PatientOrderCsvModel.CommencementDate), "Call Off Commencement Date")]
        [InlineData(4, nameof(PatientOrderCsvModel.ServiceRecipientId), "Service Recipient ID")]
        [InlineData(5, nameof(PatientOrderCsvModel.ServiceRecipientName), "Service Recipient Name")]
        [InlineData(6, nameof(PatientOrderCsvModel.ServiceRecipientItemId), "Service Recipient Item ID")]
        [InlineData(7, nameof(PatientOrderCsvModel.SupplierId), "Supplier ID")]
        [InlineData(8, nameof(PatientOrderCsvModel.SupplierName), "Supplier Name")]
        [InlineData(9, nameof(PatientOrderCsvModel.ProductId), "Product ID")]
        [InlineData(10, nameof(PatientOrderCsvModel.ProductName), "Product Name")]
        [InlineData(11, nameof(PatientOrderCsvModel.ProductType), "Product Type")]
        [InlineData(12, nameof(PatientOrderCsvModel.QuantityOrdered), "Quantity Ordered")]
        [InlineData(13, nameof(PatientOrderCsvModel.UnitOfOrder), "Unit of Order")]
        [InlineData(14, nameof(PatientOrderCsvModel.Price), "Price")]
        [InlineData(15, nameof(PatientOrderCsvModel.FundingType), "Funding Type")]
        [InlineData(16, nameof(PatientOrderCsvModel.M1Planned), "M1 planned (Delivery Date)")]
        [InlineData(17, nameof(PatientOrderCsvModel.ActualM1Date), "Actual M1 date")]
        [InlineData(18, nameof(PatientOrderCsvModel.VerficationDate), "Buyer verification date (M2)")]
        [InlineData(19, nameof(PatientOrderCsvModel.CeaseDate), "Cease Date")]
        [InlineData(20, nameof(PatientOrderCsvModel.Framework), "Framework")]
        [InlineData(21, nameof(PatientOrderCsvModel.InitialTerm), "Initial Term")]
        [InlineData(22, nameof(PatientOrderCsvModel.MaximumTerm), "Contract Length (Months)")]
        public static void ModelMap_ShouldBe_CorrectlyMapped(int index, string memberName, string name)
        {
            var map = new PatientOrderCsvModelMap();

            map.MemberMaps.Count.Should().Be(23);
            map.MemberMaps[index].Data.Member.Name.Should().Be(memberName);
            map.MemberMaps[index].Data.Names[0].Should().Be(name);
        }
    }
}
