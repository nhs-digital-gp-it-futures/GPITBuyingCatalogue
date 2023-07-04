using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class ServiceLevelAgreementDetailsModel : SolutionDisplayBaseModel
    {
        public ServiceLevelAgreementDetailsModel(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            var serviceLevelAgreement = catalogueItem.Solution.ServiceLevelAgreement;

            ServiceAvailabilityTimes = serviceLevelAgreement.ServiceHours;
            SlaContacts = serviceLevelAgreement.Contacts;
            ServiceLevels = serviceLevelAgreement.ServiceLevels;
        }

        public ICollection<ServiceAvailabilityTimes> ServiceAvailabilityTimes { get; init; }

        public ICollection<SlaContact> SlaContacts { get; init; }

        public ICollection<SlaServiceLevel> ServiceLevels { get; init; }

        public override int Index => 12;
    }
}
