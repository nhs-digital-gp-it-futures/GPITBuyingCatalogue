using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationRecipientsModel : NavBaseModel
    {
        public const string DefaultTitle = "Add service recipients";
        public const string DefaultAdvice = "Select all the organisations that will be receiving this order.";
        public const string MergerTitle = "Add merging service recipients";
        public const string MergerAdvice = "Select all the practices that will be involved in the merger. They must all be using the same Catalogue Solution.";
        public const string SplitTitle = "Service recipients splitting";
        public const string SplitAdvice = "Select all the organisations that will be involved in this split. They must all be using the same catalogue solution.";

        public SelectSublocationRecipientsModel()
        {
        }

        public SelectSublocationRecipientsModel(
            Competition competition,
            SublocationModel selectedSublocation,
            IReadOnlyList<ServiceRecipientModel> possibleServiceRecipients,
            string backLink,
            SelectionMode? selectionMode = null)
            : this(
                selectedSublocation,
                backLink,
                selectionMode)
        {
            Caption = competition.Name;
            RenderedServiceRecipients = GetRenderedSublocations(
                possibleServiceRecipients,
                selectedSublocation.ServiceRecipients);
            SelectServiceRecipients(RenderedServiceRecipients);
        }

        public SelectSublocationRecipientsModel(
            Order order,
            SublocationModel selectedSublocation,
            IReadOnlyList<ServiceRecipientModel> possibleServiceRecipients,
            string backLink,
            SelectionMode? selectionMode = null)
            : this(
                selectedSublocation,
                backLink,
                selectionMode)
        {
            Caption = order.CallOffId.ToString();
            var isMergerOrSplit = order.OrderType.MergerOrSplit;
            var isMerger = order.OrderType.ToPracticeReorganisationType == PracticeReorganisationTypeEnum.Merger;
            SetDisplayContent(isMergerOrSplit, isMerger);

            RenderedServiceRecipients = GetRenderedSublocations(
                possibleServiceRecipients,
                selectedSublocation.ServiceRecipients);

            SelectServiceRecipients(RenderedServiceRecipients);
        }

        public SelectSublocationRecipientsModel(
            Order order,
            IReadOnlyList<ServiceRecipientModel> previousServiceRecipients,
            SublocationModel selectedSublocation,
            IReadOnlyList<ServiceRecipientModel> possibleServiceRecipients,
            string backLink,
            SelectionMode? selectionMode = null)
            : this(
                selectedSublocation,
                backLink,
                selectionMode)
        {
            IsAmendment = true;
            Caption = order.CallOffId.ToString();
            var isMergerOrSplit = order.OrderType.MergerOrSplit;
            var isMerger = order.OrderType.ToPracticeReorganisationType == PracticeReorganisationTypeEnum.Merger;
            SetDisplayContent(isMergerOrSplit, isMerger);

            RenderedServiceRecipients = GetRenderedSublocations(
                possibleServiceRecipients,
                selectedSublocation.ServiceRecipients,
                previousServiceRecipients);

            SelectServiceRecipients(RenderedServiceRecipients.Where(x => !x.Hidden).ToList());
        }

        private SelectSublocationRecipientsModel(
            SublocationModel selectedSublocation,
            string backLink,
            SelectionMode? selectionMode = null)
        {
            Title = DefaultTitle;
            Advice = DefaultAdvice;
            BackLink = backLink;

            SelectionMode = selectionMode;

            SublocationName = selectedSublocation.Name;
        }

        public string SublocationName { get; init; }

        public bool? IsAmendment { get; init; }

        public List<SelectOption<string>> RenderedServiceRecipients { get; init; }

        public bool AllRecipientsSelected => RenderedServiceRecipients.All(x => x.Selected);

        public SelectionMode? SelectionMode { get; init; }

        private static IReadOnlyList<ServiceRecipientModel> MergePossibleAndExisting(
            IReadOnlyList<ServiceRecipientModel> possibleRecipients,
            IReadOnlyList<ServiceRecipientModel> existingRecipients)
        {
            return existingRecipients
                .Concat(
                    possibleRecipients.Where(nsr => existingRecipients.All(esr => esr.OdsCode != nsr.OdsCode)))
                .OrderBy(x => x.Name)
                .ToList();
        }

        private static List<SelectOption<string>> GetRenderedSublocations(
            IReadOnlyList<ServiceRecipientModel> possibleRecipients,
            IReadOnlyList<ServiceRecipientModel> existingRecipients,
            IReadOnlyList<ServiceRecipientModel> previousOrderRecipients)
        {
            IReadOnlyList<ServiceRecipientModel> possibleSelections =
                MergePossibleAndExisting(possibleRecipients, existingRecipients);

            List<SelectOption<string>> possibleSelectOptions = possibleSelections.Select(x =>
                    new SelectOption<string>(
                        x.Name,
                        x.OdsCode,
                        x.Selected,
                        previousOrderRecipients.Any(y => y.OdsCode == x.OdsCode)))
                .ToList();

            return possibleSelectOptions;
        }

        private static List<SelectOption<string>> GetRenderedSublocations(
            IReadOnlyList<ServiceRecipientModel> possibleRecipients,
            IReadOnlyList<ServiceRecipientModel> existingRecipients)
        {
            return MergePossibleAndExisting(possibleRecipients, existingRecipients)
                .Select(x => new SelectOption<string>(
                    x.Name,
                    x.OdsCode,
                    x.Selected))
                .ToList();
        }

        private void SetDisplayContent(bool isMergerOrSplit, bool isMerger)
        {
            if (isMergerOrSplit)
            {
                Title = isMerger ? MergerTitle : SplitTitle;
                Advice = isMerger ? MergerAdvice : SplitAdvice;
            }
            else
            {
                Title = DefaultTitle;
                Advice = DefaultAdvice;
            }
        }

        private void SelectServiceRecipients(
            List<SelectOption<string>> modifyList)
        {
            switch (SelectionMode)
            {
                case ServiceRecipientModels.SelectionMode.All:
                    modifyList.ForEach(x => x.Selected = true);
                    break;
                case ServiceRecipientModels.SelectionMode.None:
                    modifyList.ForEach(x => x.Selected = false);
                    break;
                case null:
                    break;
                default:
                    throw new InvalidDataException("Selection mode mot handled");
            }
        }
    }
}
