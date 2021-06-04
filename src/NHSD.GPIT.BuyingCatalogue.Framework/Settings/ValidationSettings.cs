namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public sealed class ValidationSettings
    {
        public int MaxDeliveryDateWeekOffset { get; set; }

        public int MaxDeliveryDateOffsetInDays => MaxDeliveryDateWeekOffset * 7;
    }
}
