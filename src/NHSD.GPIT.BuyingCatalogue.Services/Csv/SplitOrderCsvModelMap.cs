using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public sealed class SplitOrderCsvModelMap : ClassMapWithNames<SplitOrderCsvModel>
    {
        public static readonly KeyValuePair<string, string>[] SplitSpecificNames =
        {
            new(nameof(SplitOrderCsvModel.ServiceRecipientToSplit), "Practice to split (ODS code)"),
            new(nameof(SplitOrderCsvModel.ServiceRecipientToRetain), "Practice to be retained (ODS code)"),
        };

        public SplitOrderCsvModelMap(IEnumerable<KeyValuePair<string, string>> names)
            : base(names, SplitSpecificNames)
        {
            Map(o => o.CallOffId).Index(0).Name(GetName(nameof(SplitOrderCsvModel.CallOffId)));
            Map(o => o.OdsCode).Index(1).Name(GetName(nameof(SplitOrderCsvModel.OdsCode)));
            Map(o => o.OrganisationName).Index(2).Name(GetName(nameof(SplitOrderCsvModel.OrganisationName)));
            Map(o => o.CommencementDate).Index(3).Name(GetName(nameof(SplitOrderCsvModel.CommencementDate)));
            Map(o => o.ServiceRecipientToSplit).Index(4).Name(GetName(nameof(SplitOrderCsvModel.ServiceRecipientToSplit)));
            Map(o => o.ServiceRecipientToRetain).Index(5).Name(GetName(nameof(SplitOrderCsvModel.ServiceRecipientToRetain)));
            Map(o => o.ServiceRecipientItemId).Index(6).Name(GetName(nameof(SplitOrderCsvModel.ServiceRecipientItemId)));
            Map(o => o.SupplierId).Index(7).Name(GetName(nameof(SplitOrderCsvModel.SupplierId)));
            Map(o => o.SupplierName).Index(8).Name(GetName(nameof(SplitOrderCsvModel.SupplierName)));
            Map(o => o.ProductId).Index(9).Name(GetName(nameof(SplitOrderCsvModel.ProductId)));
            Map(o => o.ProductName).Index(10).Name(GetName(nameof(SplitOrderCsvModel.ProductName)));
            Map(o => o.ProductType).Index(11).Name(GetName(nameof(SplitOrderCsvModel.ProductType)));
            Map(o => o.QuantityOrdered).Index(12).Name(GetName(nameof(SplitOrderCsvModel.QuantityOrdered)));
            Map(o => o.UnitOfOrder).Index(13).Name(GetName(nameof(SplitOrderCsvModel.UnitOfOrder)));
            Map(o => o.UnitTime).Index(14).Name(GetName(nameof(SplitOrderCsvModel.UnitTime)));
            Map(o => o.EstimationPeriod).Index(15).Name(GetName(nameof(SplitOrderCsvModel.EstimationPeriod)));
            Map(o => o.Price).Index(16).Name(GetName(nameof(SplitOrderCsvModel.Price)));
            Map(o => o.OrderType).Index(17).Name(GetName(nameof(SplitOrderCsvModel.OrderType)));
            Map(o => o.FundingType).Index(18).Name(GetName(nameof(SplitOrderCsvModel.FundingType)));
            Map(o => o.M1Planned).Index(19).Name(GetName(nameof(SplitOrderCsvModel.M1Planned)));
            Map(o => o.ActualM1Date).Index(20).Name(GetName(nameof(SplitOrderCsvModel.ActualM1Date)));
            Map(o => o.VerficationDate).Index(21).Name(GetName(nameof(SplitOrderCsvModel.VerficationDate)));
            Map(o => o.CeaseDate).Index(22).Name(GetName(nameof(SplitOrderCsvModel.CeaseDate)));
            Map(o => o.Framework).Index(23).Name(GetName(nameof(SplitOrderCsvModel.Framework)));
            Map(o => o.InitialTerm).Index(24).Name(GetName(nameof(SplitOrderCsvModel.InitialTerm)));
            Map(o => o.MaximumTerm).Index(25).Name(GetName(nameof(SplitOrderCsvModel.MaximumTerm)));
            Map(o => o.PricingType).Index(26).Name(GetName(nameof(SplitOrderCsvModel.PricingType)));
            Map(o => o.TieredArray).Index(27).Name(GetName(nameof(SplitOrderCsvModel.TieredArray)));
        }
    }
}
