using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationRecipientsModel : NavBaseModel
    {
        private readonly SelectionMode? selectionMode;

        public SelectSublocationRecipientsModel()
        {
        }

        public SelectSublocationRecipientsModel(
            Competition competition,
            SublocationModel selectedSublocation,
            IEnumerable<ServiceRecipientModel> possibleServiceRecipients,
            IEnumerable<string> requestParameterRecipients,
            string backLinkHref,
            SelectionMode? selectionMode = null,
            bool isAmendment = false)
        {
            Title = "Add service recipients";
            Caption = competition.Name;
            Advice = "Select all the organisations that will be receiving this order";
            BackLink = backLinkHref;

            this.selectionMode = selectionMode;

            PreviouslySelected = selectedSublocation.ServiceRecipients;
            PossibleServiceRecipients = possibleServiceRecipients.ToList();

            Sublocation = selectedSublocation;

            IsAmendment = isAmendment;

            PopulateRenderedServiceRecipients();

            SelectServiceRecipients(requestParameterRecipients);
        }

        public SublocationModel Sublocation { get; set; }

        public bool HasImportedRecipients { get; set; }

        public List<ServiceRecipientModel> PreviouslySelected { get; set; }

        public bool IsAmendment { get; set; }

        private List<ServiceRecipientModel> PossibleServiceRecipients { get; }

        public List<ServiceRecipientModel> RenderedServiceRecipients { get; set; } = [];

        private void PopulateRenderedServiceRecipients()
        {
            RenderedServiceRecipients.AddRange(PreviouslySelected);
            RenderedServiceRecipients.AddRange(
                PossibleServiceRecipients.Where(
                    nsr => RenderedServiceRecipients.All(esr => esr.OdsCode != nsr.OdsCode)));
            RenderedServiceRecipients.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
        }

        private void SelectServiceRecipients(IEnumerable<string> requestParameterRecipients)
        {
            switch (selectionMode)
            {
                case SelectionMode.All:
                    RenderedServiceRecipients.ForEach(x => x.Selected = true);
                    break;
                case SelectionMode.None:
                    RenderedServiceRecipients.ForEach(x => x.Selected = false);
                    break;
                default:
                    if (requestParameterRecipients == null) return;

                    List<ServiceRecipientModel> matchingRecipients = RenderedServiceRecipients
                        .Where(x => requestParameterRecipients.Contains(x.OdsCode))
                        .ToList();
                    if (matchingRecipients.Count == 0) return;

                    matchingRecipients.ForEach(x => x.Selected = true);

                    break;
            }
        }
    }
}
