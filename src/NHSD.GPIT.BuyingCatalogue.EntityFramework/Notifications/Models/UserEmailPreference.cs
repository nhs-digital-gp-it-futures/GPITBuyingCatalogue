namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public sealed class UserEmailPreference
    {
        public int UserId { get; set; }

        public int EmailPreferenceTypeId { get; set; }

        public bool Enabled { get; set; }
    }
}
