using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Select yes if you're paying for this order in full using your GP IT Futures centrally held funding allocation")]
        public string FundingSourceOnlyGms { get; set; }
    }
}
