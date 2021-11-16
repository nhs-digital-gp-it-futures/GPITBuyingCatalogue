using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class ServiceLevelAgreementDetailsModel : SolutionDisplayBaseModel
    {
        public ServiceLevelAgreementDetailsModel(
            CatalogueItem catalogueItem,
            ICollection<ServiceAvailabilityTimes> serviceAvailabilityTimes,
            ICollection<SlaContact> slaContacts,
            ICollection<SlaServiceLevel> serviceLevels)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            if (serviceAvailabilityTimes is null)
                throw new ArgumentNullException(nameof(serviceAvailabilityTimes));

            if (slaContacts is null)
                throw new ArgumentNullException(nameof(slaContacts));

            if (serviceLevels is null)
                throw new ArgumentNullException(nameof(serviceLevels));

            ServiceAvailabilityTimes = serviceAvailabilityTimes;
            SlaContacts = slaContacts;
            ServiceLevels = serviceLevels;
        }

        public ICollection<ServiceAvailabilityTimes> ServiceAvailabilityTimes { get; init; }

        public ICollection<SlaContact> SlaContacts { get; init; }

        public ICollection<SlaServiceLevel> ServiceLevels { get; init; }

        public override int Index => 11;
    }
}
