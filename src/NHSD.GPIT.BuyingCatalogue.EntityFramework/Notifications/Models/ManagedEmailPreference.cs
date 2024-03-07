namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public sealed class ManagedEmailPreference
    {
        public int Id { get; set; } // INT NOT NULL,

        public string Name { get; set; } // NVARCHAR(20) NOT NULL,

        public bool DefaultEnabled { get; set; }
    }
}
