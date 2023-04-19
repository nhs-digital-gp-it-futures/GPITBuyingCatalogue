using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity
{
    public static class QuantityObjects
    {
        public static By QuantityInput => By.Id("Quantity");

        public static By QuantityInputError => By.Id("Quantity-error");

        public static By InputQuantityPracticeListSize => ByExtensions.DataTestId("input_quantity");

        public static By InputQuantityInput(int index) => By.Id($"ServiceRecipients_{index}__InputQuantity");

        public static By InputQuantityInputError(int index) => By.Id($"ServiceRecipients_{index}__InputQuantity-error");

        public static By InputHiddenServiceRecipientName(int index) => By.Id($"ServiceRecipients_{index}__Name");

        public static By ViewOrderItemQuantity => By.Id("order-item-quantity");

        public static By ViewServiceRecipientQuantity => By.Id("service-recipient-quantity");
    }
}
