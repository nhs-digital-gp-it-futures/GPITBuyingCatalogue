using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage
{
    public class DetermineAssociatedServiceTypeModel : NavBaseModel
    {
        public static readonly PageTitleModel PageTitle = new()
        {
            Title = TitleText,
            Advice = "Select the type of Associated Service you want to order.",
        };

        public static readonly PageTitleModel NoSuppliersForMergerAndSplitSPageTitle = new()
        {
            Title = TitleText,
            Advice = "There are currently no suppliers offering practice mergers or splits. However, you can continue to order another type of Associated Service.",
        };

        private const string TitleText = "What type of Associated Service do you want to order?";

        public DetermineAssociatedServiceTypeModel()
        {
        }

        public DetermineAssociatedServiceTypeModel(string organisationName, bool mergerEnabled, bool splitEnabled)
        {
            OrganisationName = organisationName;
            MergerEnabled = mergerEnabled;
            SplitEnabled = splitEnabled;
        }

        public string InternalOrgId { get; set; }

        public string OrganisationName { get; set; }

        public bool MergerEnabled { get; set; }

        public bool SplitEnabled { get; set; }

        public OrderTypeEnum? OrderType { get; set; }

        public IList<SelectOption<OrderTypeEnum>> AvailableOrderTypes
        {
            get
            {
                var result = new List<SelectOption<OrderTypeEnum>>();
                if (MergerEnabled)
                {
                    result.Add(new(
                        "Merger",
                        "This is where 2 or more practices join to become a single practice with a combined practice li​st size.",
                        OrderTypeEnum.AssociatedServiceMerger));
                }

                if (SplitEnabled)
                {
                    result.Add(new(
                        "Split",
                        "This is where 1 practice becomes 2 or more practices and the original practice list is shared between each party.",
                        OrderTypeEnum.AssociatedServiceSplit));
                }

                result.Add(new(
                    "Something else",
                    "This is for ordering any other type of Associated Service.",
                    OrderTypeEnum.AssociatedServiceOther));

                return result;
            }
        }

        public PageTitleModel GetPageTitle()
        {
            if (!MergerEnabled && !SplitEnabled)
            {
                return NoSuppliersForMergerAndSplitSPageTitle with { Caption = OrganisationName };
            }

            return PageTitle with { Caption = OrganisationName };
        }
    }
}
