using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public sealed class Contact : IAudited, IEquatable<Contact>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name Required")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address Required")]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telephone Number Required")]
        [StringLength(35)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public string NameOrDepartment => !string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName)
            ? FullName
            : Department;

        public string FullName => $"{FirstName} {LastName}".Trim();

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

        public Contact Clone()
        {
            return new Contact
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Phone = Phone,
                Department = Department,
            };
        }
    }
}
