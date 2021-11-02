using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public class MarketingContact : IAudited
    {
        public int Id { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        [StringLength(35)]
        public string FirstName { get; set; }

        [StringLength(35)]
        public string LastName { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(35)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public virtual bool IsEmpty() =>
            string.IsNullOrWhiteSpace(FirstName)
            && string.IsNullOrWhiteSpace(LastName)
            && string.IsNullOrWhiteSpace(Department)
            && string.IsNullOrWhiteSpace(PhoneNumber)
            && string.IsNullOrWhiteSpace(Email);

        public virtual void UpdateFrom(MarketingContact sourceContact)
        {
            if (sourceContact == null)
                throw new ArgumentNullException(nameof(sourceContact));

            Department = sourceContact.Department;
            Email = sourceContact.Email;
            FirstName = sourceContact.FirstName;
            LastName = sourceContact.LastName;
            LastUpdated = sourceContact.LastUpdated;
            PhoneNumber = sourceContact.PhoneNumber;
        }

        public virtual bool NewAndValid() => Id == default && !IsEmpty();
    }
}
