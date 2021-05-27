using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution
{
    public class SolutionStatusModel : MarketingDisplayBaseModel
    {
        public string Description { get; set; }

        public string Framework { get; set; }

        public override DateTime LastReviewed { get; set; }

        public override PaginationFooterModel PaginationFooter { get; set; }

        public override string Section { get; set; }

        public override string SolutionId { get; set; }

        public string SolutionName { get; set; }

        public string Summary { get; set; }

        public string SupplierName { get; set; }
    }
}
