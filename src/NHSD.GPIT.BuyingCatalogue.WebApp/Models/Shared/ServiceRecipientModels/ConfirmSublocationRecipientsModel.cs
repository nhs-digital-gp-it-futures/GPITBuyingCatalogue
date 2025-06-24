using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class ConfirmSublocationRecipientsModel : NavBaseModel
    {
        private const string OrderAdvice =
            "Review the organisations you’ve selected to receive the items you’re ordering.";

        public ConfirmSublocationRecipientsModel()
        {
        }

        public ConfirmSublocationRecipientsModel(
            Competition competition,
            string backLinkUrl,
            string continueLinkUrl)
            : this(backLinkUrl, continueLinkUrl)
        {
            Caption = competition.Name;
            Advice = "Review the organisations you've selected to receive the winning solution for this competition.";

            Sublocations = competition.CompetitionSublocations
                .Select(cs => new SublocationModel(cs, false))
                .ToArray();
        }

        public ConfirmSublocationRecipientsModel(
            Order order,
            string backLinkUrl,
            string continueLinkUrl)
            : this(backLinkUrl, continueLinkUrl)
        {
            Caption = order.CallOffId.ToString();
            Advice = OrderAdvice;

            Sublocations = order.OrderSublocations
                .Select(os => new SublocationModel(os, false))
                .ToArray();
        }

        public ConfirmSublocationRecipientsModel(
            OrderWrapper wrapper,
            string backLinkUrl,
            string continueLinkUrl)
            : this(backLinkUrl, continueLinkUrl)
        {
            Caption = wrapper.Order.CallOffId.ToString();
            Advice = OrderAdvice;

            IEnumerable<OrderSublocation> sublocationsWithNewRecipients = wrapper.Order.OrderSublocations.Where(x =>
            {
                OrderSublocation previousSublocation = wrapper.Previous?.OrderSublocations
                    .FirstOrDefault(y => y.SublocationOdsCode == x.SublocationOdsCode);

                return x.SublocationRecipients.Count > previousSublocation?.SublocationRecipients.Count;
            });

            Sublocations = sublocationsWithNewRecipients
                .Select(sl =>
                {
                    List<ServiceRecipientModel> serviceRecipients = sl.SublocationRecipients
                        .Where(x =>
                        {
                            OrderSublocation previousSublocation = wrapper.Previous?.OrderSublocations
                                .FirstOrDefault(y => y.SublocationOdsCode == x.ParentSublocationOdsCode);

                            if (previousSublocation is null)
                            {
                                return true;
                            }

                            return previousSublocation
                                .SublocationRecipients.All(y =>
                                    x.RecipientOdsCode != y.RecipientOdsCode);
                        })
                        .Select(x => new ServiceRecipientModel(x, false))
                        .ToList();

                    return new SublocationModel
                    {
                        Name = sl.SublocationOrganisation.Name,
                        OdsCode = sl.SublocationOdsCode,
                        ServiceRecipients = serviceRecipients,
                        ServiceRecipientCount = serviceRecipients.Count,
                    };
                })
                .ToArray();
        }

        private ConfirmSublocationRecipientsModel(
            string backLinkUrl,
            string continueLinkUrl)
        {
            Title = "Confirm service recipients";

            BackLink = backLinkUrl;

            AddRemoveRecipientsLink = BackLink;
            SaveAndContinueLink = continueLinkUrl;
        }

        public string AddRemoveRecipientsLink { get; init; }

        public string SaveAndContinueLink { get; init; }

        public IReadOnlyCollection<SublocationModel> Sublocations { get; init; }
    }
}
