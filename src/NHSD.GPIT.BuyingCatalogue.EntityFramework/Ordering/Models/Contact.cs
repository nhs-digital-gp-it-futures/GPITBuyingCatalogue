using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public sealed partial class Contact : IAudited
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
