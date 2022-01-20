using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource
{
    public sealed class FundingSourceModel : OrderingBaseModel
    {
        public FundingSourceModel()
        {
        }

        public FundingSourceModel(string odsCode, CallOffId callOffId, bool? fundingSourceOnlyGms)
        {
            Title = $"Funding source for {callOffId}";
            FundingSourceOnlyGms = fundingSourceOnlyGms.ToYesNo();
        }

        public string FundingSourceOnlyGms { get; set; }
    }
}
