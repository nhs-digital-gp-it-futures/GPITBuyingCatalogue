namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public sealed class EventType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? EmailPreferenceTypeId { get; set; }

        public EmailPreferenceType EmailPreferenceType { get; set; }
    }
}
