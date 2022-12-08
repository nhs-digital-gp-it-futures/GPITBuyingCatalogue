using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public partial class Contact : IEquatable<Contact>
    {
        public bool Equals(Contact other)
        {
            if (other == null)
            {
                return false;
            }

            return (FirstName ?? string.Empty).Equals(other.FirstName ?? string.Empty)
                && (LastName ?? string.Empty).Equals(other.LastName ?? string.Empty)
                && (Email ?? string.Empty).Equals(other.Email ?? string.Empty)
                && (Phone ?? string.Empty).Equals(other.Phone ?? string.Empty)
                && (Department ?? string.Empty).Equals(other.Department ?? string.Empty);
        }
    }
}
