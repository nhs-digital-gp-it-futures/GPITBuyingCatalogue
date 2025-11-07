using System;
using System.Collections.Generic;
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
            List<ServiceRecipientModel> selectedRecipients,
            ServiceRecipientModel practiceReorganisationRecipient,
            string backlink,
            string addRemoveRecipientsLink)
        {
            GetTitleAndAdviceFromOrderType(orderType);
            Caption = $"Order {callOffId}";
            BackLink = backlink;
            AddRemoveRecipientsLink = addRemoveRecipientsLink;

            OrderType = orderType;
            SelectedRecipients = selectedRecipients;
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

        public ServiceRecipientModel PracticeReorganisationRecipient { get; set; }

        public List<ServiceRecipientModel> SelectedRecipients { get; set; } = [];

        private void GetTitleAndAdviceFromOrderType(OrderType orderType)
        {
            Title = "Confirm service recipients";
            Advice = orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceSplit => "Review the practices involved in the split you’re ordering.",
                OrderTypeEnum.AssociatedServiceMerger =>
                    Advice = "Review the practices involved in the merger you’re ordering.",
                _ => throw new ArgumentOutOfRangeException(nameof(orderType)),
            };
        }
    }
}
