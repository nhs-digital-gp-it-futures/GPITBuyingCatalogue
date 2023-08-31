using CsvHelper.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public sealed class FullOrderCsvModelMap : ClassMap<FullOrderCsvModel>
    {
        public FullOrderCsvModelMap()
        {
            Map(o => o.CallOffId).Index(0).Name("Call Off Agreement ID");
            Map(o => o.OdsCode).Index(1).Name("Call Off Ordering Party ID");
            Map(o => o.OrganisationName).Index(2).Name("Call Off Ordering Party Name");
            Map(o => o.CommencementDate).Index(3).Name("Call Off Commencement Date");
            Map(o => o.ServiceRecipientId).Index(4).Name("Service Recipient ID");
            Map(o => o.ServiceRecipientName).Index(5).Name("Service Recipient Name");
            Map(o => o.ServiceRecipientItemId).Index(6).Name("Service Recipient Item ID");
            Map(o => o.SupplierId).Index(7).Name("Supplier ID");
            Map(o => o.SupplierName).Index(8).Name("Supplier Name");
            Map(o => o.ProductId).Index(9).Name("Product ID");
            Map(o => o.ProductName).Index(10).Name("Product Name");
            Map(o => o.ProductType).Index(11).Name("Product Type");
            Map(o => o.QuantityOrdered).Index(12).Name("Quantity Ordered");
            Map(o => o.UnitOfOrder).Index(13).Name("Unit of Order");
            Map(o => o.UnitTime).Index(14).Name("Unit Time");
            Map(o => o.EstimationPeriod).Index(15).Name("Estimation Period");
            Map(o => o.Price).Index(16).Name("Price");
            Map(o => o.OrderType).Index(17).Name("Order Type");
            Map(o => o.FundingType).Index(18).Name("Funding Type");
            Map(o => o.M1Planned).Index(19).Name("M1 planned (Delivery Date)");
            Map(o => o.ActualM1Date).Index(20).Name("Actual M1 date");
            Map(o => o.VerficationDate).Index(21).Name("Buyer verification date (M2)");
            Map(o => o.CeaseDate).Index(22).Name("Forecast Cease Date");
            Map(o => o.Framework).Index(23).Name("Framework");
            Map(o => o.InitialTerm).Index(24).Name("Initial Term");
            Map(o => o.MaximumTerm).Index(25).Name("Contract Length (Months)");
        }
    }
}
