using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class ConfirmChangesModel : NavBaseModel
    {
        public const string TitleText = "Confirm service recipients";
        private string addRemoveRecipientsLink;

        public ConfirmChangesModel()
        {
        }

        public ConfirmChangesModel(
            CallOffId callOffId,
            OrderType orderType,
            List<OrderSublocationRecipient> selectedRecipients,
            OrderSublocationRecipient practiceReorganisationRecipient)
        {
            GetTitleAndAdviceFromOrderType(orderType);
            Caption = $"Order {callOffId}";

            OrderType = orderType;
            Selected = selectedRecipients;
            PracticeReorganisationRecipient = practiceReorganisationRecipient;
        }

        public ConfirmChangesModel(Organisation organisation)
        {
            Title = $"{TitleText} for {organisation.Name} ({organisation.ExternalIdentifier})";
        }

        public string AddRemoveRecipientsLink
        {
            get
            {
                return addRemoveRecipientsLink == null
                    ? BackLink
                    : addRemoveRecipientsLink;
            }

            set
            {
                addRemoveRecipientsLink = value;
            }
        }

        public OrderType OrderType { get; set; }

        public OrderSublocationRecipient PracticeReorganisationRecipient { get; set; }

        public List<OrderSublocationRecipient> Selected { get; set; } = [];

        private void GetTitleAndAdviceFromOrderType(OrderType orderType)
        {
            Title = TitleText;

            var processType = orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceSplit => "split",
                OrderTypeEnum.AssociatedServiceMerger => "merger",
                _ => throw new ArgumentOutOfRangeException(nameof(orderType)),
            };
            Advice = $"Review the practices involved in the {processType} you're ordering.";
        }
    }
}
