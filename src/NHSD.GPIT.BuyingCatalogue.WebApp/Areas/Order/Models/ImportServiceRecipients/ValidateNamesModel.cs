using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ImportServiceRecipients;

public class ValidateNamesModel : NavBaseModel
{
    public ValidateNamesModel()
    {
    }

    public ValidateNamesModel(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        string catalogueItemName,
        ServiceRecipientImportMode importMode,
        IEnumerable<ServiceRecipientImportModel> importedServiceRecipients,
        IEnumerable<NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient> mismatchedServiceRecipients)
    {
        InternalOrgId = internalOrgId;
        CallOffId = callOffId;
        CatalogueItemId = catalogueItemId;
        CatalogueItemName = catalogueItemName;
        ImportMode = importMode;

        NameDiscrepancies = mismatchedServiceRecipients.Select(
                r => new ServiceRecipientNameDiscrepancy(
                    importedServiceRecipients.First(x => string.Equals(x.OdsCode, r.OrgId)).Organisation,
                    r.Name,
                    r.OrgId))
            .ToList();
    }

    public string InternalOrgId { get; set; }

    public CallOffId CallOffId { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public string CatalogueItemName { get; set; }

    public ServiceRecipientImportMode ImportMode { get; set; }

    public IList<ServiceRecipientNameDiscrepancy> NameDiscrepancies { get; set; }

    public class ServiceRecipientNameDiscrepancy
    {
        public ServiceRecipientNameDiscrepancy()
        {
        }

        public ServiceRecipientNameDiscrepancy(
            string expectedName,
            string actualName,
            string odsCode)
        {
            ExpectedName = expectedName;
            ActualName = actualName;
            OdsCode = odsCode;
        }

        public string ActualName { get; set; }

        public string ExpectedName { get; set; }

        public string OdsCode { get; set; }
    }
}
