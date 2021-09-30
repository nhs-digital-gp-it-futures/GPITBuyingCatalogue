using System.Collections.Generic;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class PublicationStatusExtensions
    {
        private static readonly IDictionary<PublicationStatus, List<PublicationStatus>> PublicationStatuses
               = new Dictionary<PublicationStatus, List<PublicationStatus>>
               {
                   [PublicationStatus.Draft] = new(2)
                   {
                       PublicationStatus.Draft,
                       PublicationStatus.Published,
                   },
                   [PublicationStatus.Published] = new(3)
                   {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Unpublished,
                   },
                   [PublicationStatus.InRemediation] = new(4)
                   {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Suspended,
                       PublicationStatus.Unpublished,
                   },
                   [PublicationStatus.Suspended] = new(4)
                   {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Suspended,
                       PublicationStatus.Unpublished,
                   },
                   [PublicationStatus.Unpublished] = new(2)
                   {
                       PublicationStatus.Published,
                       PublicationStatus.Unpublished,
                   },
               };

        public static string Name(this PublicationStatus publicationStatus) => publicationStatus.AsString(EnumFormat.DisplayName);

        public static string EnumMemberName(this PublicationStatus publicationStatus) => publicationStatus.ToString();

        public static string Description(this PublicationStatus publicationStatus) => publicationStatus.AsString(EnumFormat.Description);

        public static IList<PublicationStatus> GetAvailablePublicationStatuses(this PublicationStatus publicationStatus)
        {
            if (!PublicationStatuses.TryGetValue(publicationStatus, out var availablePublicationStatuses))
            {
                throw new KeyNotFoundException($"{publicationStatus} is not a valid key");
            }

            return availablePublicationStatuses;
        }
    }
}
