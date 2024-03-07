namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public sealed class Event
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ManagedEmailPreferenceId { get; set; }

        public ManagedEmailPreference ManagedEmailPreference { get; set; }
    }
}
