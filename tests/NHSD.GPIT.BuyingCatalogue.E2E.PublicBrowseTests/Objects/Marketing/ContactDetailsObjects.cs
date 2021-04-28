using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing
{
    internal static class ContactDetailsObjects
    {
        public static By ContactFirstName(int index) => By.Id($"Contact{index}_FirstName");

        public static By ContactLastName(int index) => By.Id($"Contact{index}_LastName");

        public static By ContactPhoneNumber(int index) => By.Id($"Contact{index}_PhoneNumber");

        public static By ContactEmailAddress(int index) => By.Id($"Contact{index}_Email");

        public static By ContactJobSector(int index) => By.Id($"Contact{index}_Department");
    }
}
