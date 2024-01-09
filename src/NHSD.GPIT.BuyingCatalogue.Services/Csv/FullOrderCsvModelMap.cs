using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public sealed class FullOrderCsvModelMap : ClassMapWithNames<FullOrderCsvModel>
    {
        public static readonly KeyValuePair<string, string>[] Names =
        {
            new(nameof(FullOrderCsvModel.CallOffId), "Call Off Agreement ID"),
            new(nameof(FullOrderCsvModel.OdsCode), "Call Off Ordering Party ID"),
            new(nameof(FullOrderCsvModel.OrganisationName), "Call Off Ordering Party Name"),
            new(nameof(FullOrderCsvModel.CommencementDate), "Call Off Commencement Date"),
            new(nameof(FullOrderCsvModel.ServiceRecipientId), "Service Recipient ID"),
            new(nameof(FullOrderCsvModel.ServiceRecipientName), "Service Recipient Name"),
            new(nameof(FullOrderCsvModel.ServiceRecipientItemId), "Service Recipient Item ID"),
            new(nameof(FullOrderCsvModel.SupplierId), "Supplier ID"),
            new(nameof(FullOrderCsvModel.SupplierName), "Supplier Name"),
            new(nameof(FullOrderCsvModel.ProductId), "Product ID"),
            new(nameof(FullOrderCsvModel.ProductName), "Product Name"),
            new(nameof(FullOrderCsvModel.ProductType), "Product Type"),
            new(nameof(FullOrderCsvModel.QuantityOrdered), "Quantity Ordered"),
            new(nameof(FullOrderCsvModel.UnitOfOrder), "Unit of Order"),
            new(nameof(FullOrderCsvModel.UnitTime), "Unit Time"),
            new(nameof(FullOrderCsvModel.EstimationPeriod), "Estimation Period"),
            new(nameof(FullOrderCsvModel.Price), "Price"),
            new(nameof(FullOrderCsvModel.OrderType), "Order Type"),
            new(nameof(FullOrderCsvModel.FundingType), "Funding Type"),
            new(nameof(FullOrderCsvModel.M1Planned), "M1 planned (Delivery Date)"),
            new(nameof(FullOrderCsvModel.ActualM1Date), "Actual M1 date"),
            new(nameof(FullOrderCsvModel.VerficationDate), "Buyer verification date (M2)"),
            new(nameof(FullOrderCsvModel.CeaseDate), "Cease Date"),
            new(nameof(FullOrderCsvModel.Framework), "Framework"),
            new(nameof(FullOrderCsvModel.InitialTerm), "Initial Term"),
            new(nameof(FullOrderCsvModel.MaximumTerm), "Contract Length (Months)"),
            new(nameof(FullOrderCsvModel.PricingType), "Pricing Type"),
            new(nameof(FullOrderCsvModel.TieredArray), "Tiered Array"),
        };

        public FullOrderCsvModelMap()
            : base(new Dictionary<string, string>(Names).AsReadOnly())
        {
            Map(o => o.CallOffId).Index(0).Name(GetName(nameof(FullOrderCsvModel.CallOffId)));
            Map(o => o.OdsCode).Index(1).Name(GetName(nameof(FullOrderCsvModel.OdsCode)));
            Map(o => o.OrganisationName).Index(2).Name(GetName(nameof(FullOrderCsvModel.OrganisationName)));
            Map(o => o.CommencementDate).Index(3).Name(GetName(nameof(FullOrderCsvModel.CommencementDate)));
            Map(o => o.ServiceRecipientId).Index(4).Name(GetName(nameof(FullOrderCsvModel.ServiceRecipientId)));
            Map(o => o.ServiceRecipientName).Index(5).Name(GetName(nameof(FullOrderCsvModel.ServiceRecipientName)));
            Map(o => o.ServiceRecipientItemId).Index(6).Name(GetName(nameof(FullOrderCsvModel.ServiceRecipientItemId)));
            Map(o => o.SupplierId).Index(7).Name(GetName(nameof(FullOrderCsvModel.SupplierId)));
            Map(o => o.SupplierName).Index(8).Name(GetName(nameof(FullOrderCsvModel.SupplierName)));
            Map(o => o.ProductId).Index(9).Name(GetName(nameof(FullOrderCsvModel.ProductId)));
            Map(o => o.ProductName).Index(10).Name(GetName(nameof(FullOrderCsvModel.ProductName)));
            Map(o => o.ProductType).Index(11).Name(GetName(nameof(FullOrderCsvModel.ProductType)));
            Map(o => o.QuantityOrdered).Index(12).Name(GetName(nameof(FullOrderCsvModel.QuantityOrdered)));
            Map(o => o.UnitOfOrder).Index(13).Name(GetName(nameof(FullOrderCsvModel.UnitOfOrder)));
            Map(o => o.UnitTime).Index(14).Name(GetName(nameof(FullOrderCsvModel.UnitTime)));
            Map(o => o.EstimationPeriod).Index(15).Name(GetName(nameof(FullOrderCsvModel.EstimationPeriod)));
            Map(o => o.Price).Index(16).Name(GetName(nameof(FullOrderCsvModel.Price)));
            Map(o => o.OrderType).Index(17).Name(GetName(nameof(FullOrderCsvModel.OrderType)));
            Map(o => o.FundingType).Index(18).Name(GetName(nameof(FullOrderCsvModel.FundingType)));
            Map(o => o.M1Planned).Index(19).Name(GetName(nameof(FullOrderCsvModel.M1Planned)));
            Map(o => o.ActualM1Date).Index(20).Name(GetName(nameof(FullOrderCsvModel.ActualM1Date)));
            Map(o => o.VerficationDate).Index(21).Name(GetName(nameof(FullOrderCsvModel.VerficationDate)));
            Map(o => o.CeaseDate).Index(22).Name(GetName(nameof(FullOrderCsvModel.CeaseDate)));
            Map(o => o.Framework).Index(23).Name(GetName(nameof(FullOrderCsvModel.Framework)));
            Map(o => o.InitialTerm).Index(24).Name(GetName(nameof(FullOrderCsvModel.InitialTerm)));
            Map(o => o.MaximumTerm).Index(25).Name(GetName(nameof(FullOrderCsvModel.MaximumTerm)));
            Map(o => o.PricingType).Index(26).Name(GetName(nameof(FullOrderCsvModel.PricingType)));
            Map(o => o.TieredArray).Index(27).Name(GetName(nameof(FullOrderCsvModel.TieredArray)));
        }
    }
}
