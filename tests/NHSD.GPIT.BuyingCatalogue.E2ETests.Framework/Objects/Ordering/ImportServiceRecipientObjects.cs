using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;

public static class ImportServiceRecipientObjects
{
    public static By FileInput => By.Id(nameof(ImportServiceRecipientModel.File));

    public static By OdsCodeTable => ByExtensions.DataTestId("validate-ods-table");

    public static By OrganisationNameTable => ByExtensions.DataTestId("validate-names-table");
}
