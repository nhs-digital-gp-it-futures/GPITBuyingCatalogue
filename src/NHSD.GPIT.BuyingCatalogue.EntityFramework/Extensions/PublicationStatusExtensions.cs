using System.Collections.Generic;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class PublicationStatusExtensions
    {
        private static readonly IDictionary<PublicationStatus, IReadOnlyList<PublicationStatus>> CatalogueSolutionPublicationStatuses
               = new Dictionary<PublicationStatus, IReadOnlyList<PublicationStatus>>
               {
                   [PublicationStatus.Draft] = new List<PublicationStatus>(2)
                   {
                       PublicationStatus.Draft,
                       PublicationStatus.Published,
                   },
                   [PublicationStatus.Published] = new List<PublicationStatus>(3)
                   {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Unpublished,
                   },
                   [PublicationStatus.InRemediation] = new List<PublicationStatus>(4)
                   {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Suspended,
                       PublicationStatus.Unpublished,
                   },
                   [PublicationStatus.Suspended] = new List<PublicationStatus>(4)
                   {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Suspended,
                       PublicationStatus.Unpublished,
                   },
                   [PublicationStatus.Unpublished] = new List<PublicationStatus>(2)
                   {
                       PublicationStatus.Published,
                       PublicationStatus.Unpublished,
                   },
               };

        private static readonly IDictionary<PublicationStatus, IReadOnlyList<PublicationStatus>> ServicesPublicationStatuses
                   = new Dictionary<PublicationStatus, IReadOnlyList<PublicationStatus>>
                   {
                       [PublicationStatus.Draft] = new List<PublicationStatus>(2)
                       {
                           PublicationStatus.Draft,
                           PublicationStatus.Published,
                       },
                       [PublicationStatus.Published] = new List<PublicationStatus>(2)
                       {
                           PublicationStatus.Published,
                           PublicationStatus.Unpublished,
                       },
                       [PublicationStatus.Unpublished] = new List<PublicationStatus>(2)
                       {
                           PublicationStatus.Published,
                           PublicationStatus.Unpublished,
                       },
                   };

        public static string Name(this PublicationStatus publicationStatus) => publicationStatus.AsString(EnumFormat.DisplayName);

        public static string EnumMemberName(this PublicationStatus publicationStatus) => publicationStatus.ToString();

        public static string Description(this PublicationStatus publicationStatus) => publicationStatus.AsString(EnumFormat.Description);

        public static IReadOnlyList<PublicationStatus> GetAvailablePublicationStatuses(this PublicationStatus publicationStatus, CatalogueItemType catalogueItemType)
        {
            var publicationStatuses = catalogueItemType == CatalogueItemType.Solution ? CatalogueSolutionPublicationStatuses : ServicesPublicationStatuses;
            if (!publicationStatuses.TryGetValue(publicationStatus, out var availablePublicationStatuses))
            {
                throw new KeyNotFoundException($"{publicationStatus} is not a valid key");
            }

            return availablePublicationStatuses;
        }
    }
}
