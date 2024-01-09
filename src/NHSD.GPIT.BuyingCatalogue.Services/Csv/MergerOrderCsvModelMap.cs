using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public sealed class MergerOrderCsvModelMap : ClassMapWithNames<MergerOrderCsvModel>
    {
        public static readonly KeyValuePair<string, string>[] MergerSpecificNames =
        {
            new(nameof(MergerOrderCsvModel.ServiceRecipientToClose), "Practice to close / become branch site (ODS code)"),
            new(nameof(MergerOrderCsvModel.ServiceRecipientToRetain), "Practice to be retained (ODS code)"),
        };

        public MergerOrderCsvModelMap(IEnumerable<KeyValuePair<string, string>> names)
            : base(names, MergerSpecificNames)
        {
            Map(o => o.CallOffId).Index(0).Name(GetName(nameof(MergerOrderCsvModel.CallOffId)));
            Map(o => o.OdsCode).Index(1).Name(GetName(nameof(MergerOrderCsvModel.OdsCode)));
            Map(o => o.OrganisationName).Index(2).Name(GetName(nameof(MergerOrderCsvModel.OrganisationName)));
            Map(o => o.CommencementDate).Index(3).Name(GetName(nameof(MergerOrderCsvModel.CommencementDate)));
            Map(o => o.ServiceRecipientToClose).Index(4).Name(GetName(nameof(MergerOrderCsvModel.ServiceRecipientToClose)));
            Map(o => o.ServiceRecipientToRetain).Index(5).Name(GetName(nameof(MergerOrderCsvModel.ServiceRecipientToRetain)));
            Map(o => o.ServiceRecipientItemId).Index(6).Name(GetName(nameof(MergerOrderCsvModel.ServiceRecipientItemId)));
            Map(o => o.SupplierId).Index(7).Name(GetName(nameof(MergerOrderCsvModel.SupplierId)));
            Map(o => o.SupplierName).Index(8).Name(GetName(nameof(MergerOrderCsvModel.SupplierName)));
            Map(o => o.ProductId).Index(9).Name(GetName(nameof(MergerOrderCsvModel.ProductId)));
            Map(o => o.ProductName).Index(10).Name(GetName(nameof(MergerOrderCsvModel.ProductName)));
            Map(o => o.ProductType).Index(11).Name(GetName(nameof(MergerOrderCsvModel.ProductType)));
            Map(o => o.QuantityOrdered).Index(12).Name(GetName(nameof(MergerOrderCsvModel.QuantityOrdered)));
            Map(o => o.UnitOfOrder).Index(13).Name(GetName(nameof(MergerOrderCsvModel.UnitOfOrder)));
            Map(o => o.UnitTime).Index(14).Name(GetName(nameof(MergerOrderCsvModel.UnitTime)));
            Map(o => o.EstimationPeriod).Index(15).Name(GetName(nameof(MergerOrderCsvModel.EstimationPeriod)));
            Map(o => o.Price).Index(16).Name(GetName(nameof(MergerOrderCsvModel.Price)));
            Map(o => o.OrderType).Index(17).Name(GetName(nameof(MergerOrderCsvModel.OrderType)));
            Map(o => o.FundingType).Index(18).Name(GetName(nameof(MergerOrderCsvModel.FundingType)));
            Map(o => o.M1Planned).Index(19).Name(GetName(nameof(MergerOrderCsvModel.M1Planned)));
            Map(o => o.ActualM1Date).Index(20).Name(GetName(nameof(MergerOrderCsvModel.ActualM1Date)));
            Map(o => o.VerficationDate).Index(21).Name(GetName(nameof(MergerOrderCsvModel.VerficationDate)));
            Map(o => o.CeaseDate).Index(22).Name(GetName(nameof(MergerOrderCsvModel.CeaseDate)));
            Map(o => o.Framework).Index(23).Name(GetName(nameof(MergerOrderCsvModel.Framework)));
            Map(o => o.InitialTerm).Index(24).Name(GetName(nameof(MergerOrderCsvModel.InitialTerm)));
            Map(o => o.MaximumTerm).Index(25).Name(GetName(nameof(MergerOrderCsvModel.MaximumTerm)));
            Map(o => o.PricingType).Index(26).Name(GetName(nameof(MergerOrderCsvModel.PricingType)));
            Map(o => o.TieredArray).Index(27).Name(GetName(nameof(MergerOrderCsvModel.TieredArray)));
        }
    }
}
