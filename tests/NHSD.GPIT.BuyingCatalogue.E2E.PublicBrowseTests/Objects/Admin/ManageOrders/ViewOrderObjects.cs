using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ManageOrders
{
    internal static class ViewOrderObjects
    {
        internal static By OrganisationSection => ByExtensions.DataTestId("view-order-organisation");

        internal static By DescriptionSection => ByExtensions.DataTestId("view-order-description");

        internal static By LastUpdatedBySection => ByExtensions.DataTestId("view-order-lastupdatedby");

        internal static By SupplierSection => ByExtensions.DataTestId("view-order-supplier");

        internal static By StatusSection => ByExtensions.DataTestId("view-order-status");

        internal static By DownloadPdf => By.LinkText("Download order summary PDF");

        internal static By FullOrderCsv => By.LinkText("Download full order CSV");

        internal static By PatientOnlyCsv => By.LinkText("Download patient only CSV");

        internal static By OrdersItemsTable => ByExtensions.DataTestId("order-items-table");

        internal static By NoOrderItems => ByExtensions.DataTestId("no-order-items");

        internal static By FundingType => ByExtensions.DataTestId("funding-type");
    }
}
