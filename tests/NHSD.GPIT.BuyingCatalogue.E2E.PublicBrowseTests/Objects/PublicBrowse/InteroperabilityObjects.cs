using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class InteroperabilityObjects
    {
        internal static By TableBulk => ByExtensions.DataTestId("table-integrations-bulk");

        internal static By TableTransactional => ByExtensions.DataTestId("table-integrations-transactional");

        internal static By TablePatientFacing => ByExtensions.DataTestId("table-integrations-patient-facing");

        internal static By TableHtmlView => ByExtensions.DataTestId("table-gpconnect-html-view");

        internal static By TableAppointmentBooking => ByExtensions.DataTestId("table-gpconnect-appointment-booking");

        internal static By TableStructuredRecord => ByExtensions.DataTestId("table-gpconnect-structured-record");

        internal static By IMIntegrationsHeading => By.XPath("//h3[text()='IM1 integrations']");

        internal static By GpConnectHeading => By.XPath("//h3[text()='GP Connect integrations']");
    }
}
