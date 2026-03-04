using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public sealed class EditSlaTypeConfirmationModel : NavBaseModel
    {
        public EditSlaTypeConfirmationModel()
        {
        }

        public EditSlaTypeConfirmationModel(CatalogueItem catalogueItem)
        {
            ArgumentNullException.ThrowIfNull(catalogueItem);

            SolutionName = catalogueItem.Name;

            var newSlaType = catalogueItem.Solution.ServiceLevelAgreement.SlaType == SlaType.Type1 ? SlaType.Type2 : SlaType.Type1;

            Advice = $"If you change from a {catalogueItem.Solution.ServiceLevelAgreement.SlaType} to a {newSlaType} catalogue solution, the SLA information that was previously entered will be replaced";
        }

        public string SolutionName { get; set; }
    }
}
