using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public sealed class EmailPreferenceType
    {
        public EmailPreferenceType()
        {
            UserPreferences = new HashSet<UserEmailPreference>();
        }

        public int Id { get; set; }

        public EmailPreferenceTypeEnum EmailPreferenceTypeAsEnum => (EmailPreferenceTypeEnum)Id;

        public string Name { get; set; }

        public bool DefaultEnabled { get; set; }

        public ICollection<UserEmailPreference> UserPreferences { get; set; }
    }
}
