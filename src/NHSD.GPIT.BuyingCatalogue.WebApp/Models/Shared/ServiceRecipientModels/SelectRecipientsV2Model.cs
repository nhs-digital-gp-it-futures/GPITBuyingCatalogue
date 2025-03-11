using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectRecipientsV2Model : NavBaseModel
    {
        private readonly SelectionMode? selectionMode;

        public SelectRecipientsV2Model(
            Competition competition,
            SublocationModel selectedSublocation,
            IEnumerable<ServiceRecipientModel> possibleServiceRecipients,
            IEnumerable<string> requestSelectedRecipients,
            SelectionMode? selectionMode = null,
            bool isAmendment = false)
        {
            Title = "Add service recipients";
            Caption = competition.Name;
            Advice = "Select all the organisations that will be receiving this order";

            this.selectionMode = selectionMode;

            Organisation organisation = competition.Organisation;

            OrganisationName = organisation.Name;
            OrganisationType = organisation.OrganisationType.GetValueOrDefault();
            PreviouslySelected = selectedSublocation.ServiceRecipients;
            PossibleServiceRecipients = possibleServiceRecipients.ToList();

            List<string> previouslySelectedAsString = PreviouslySelected.Select(x => x.OdsCode).ToList();

            Sublocation = selectedSublocation;

            IsAmendment = isAmendment;

            SelectServiceRecipients(previouslySelectedAsString, requestSelectedRecipients);
        }

        public string OrganisationName { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public SublocationModel Sublocation { get; set; }

        public ServiceRecipientModel[] SearchRecipients => Sublocation.ServiceRecipients
            .Select(y => new ServiceRecipientModel { Name = y.Name, OdsCode = y.OdsCode })
            .OrderBy(x => x.Name)
            .ToArray();

        public bool HasImportedRecipients { get; set; }

        public List<ServiceRecipientModel> PreviouslySelected { get; set; }

        public bool ShouldExpand { get; set; }

        public int? SelectAtLeast { get; set; }

        public bool IsAmendment { get; set; }

        public List<ServiceRecipientModel> PossibleServiceRecipients { get; set; } = [];

        public IEnumerable<ServiceRecipientModel> GetServiceRecipients()
        {
            return PossibleServiceRecipients;
        }

        public IEnumerable<ServiceRecipientModel> GetSelectedServiceRecipients()
        {
            return GetServiceRecipients().Where(x => x.Selected);
        }

        public bool HasSelectedRecipients()
        {
            return GetSelectedServiceRecipients().Any();
        }

        private void SelectServiceRecipients(
            IEnumerable<string> existingRecipients,
            IEnumerable<string> recipients)
        {
            switch (selectionMode)
            {
                case SelectionMode.All:
                    GetServiceRecipients().ToList().ForEach(x => x.Selected = true);
                    break;
                case SelectionMode.None:
                    GetServiceRecipients().ToList().ForEach(x => x.Selected = false);
                    break;
                default:
                    if (recipients == null) return;

                    var enumeratedRecipients = recipients.ToArray();
                    var recipientsToSelect = enumeratedRecipients.Any()
                        ? enumeratedRecipients.ToArray()
                        : existingRecipients.ToArray();

                    List<ServiceRecipientModel> matchingRecipients = GetServiceRecipients()
                        .Where(x => recipientsToSelect.Contains(x.OdsCode))
                        .ToList();
                    if (!matchingRecipients.Any())
                        return;

                    matchingRecipients.ForEach(x => x.Selected = true);

                    var allSelected = GetServiceRecipients().All(x => x.Selected);

                    break;
            }
        }
    }
}
