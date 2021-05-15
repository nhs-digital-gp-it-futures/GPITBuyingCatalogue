using System;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class MarketingContact
    {
        public int Id { get; set; }
        public string SolutionId { get; set; }

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
        public Guid LastUpdatedBy { get; set; }

        public virtual Solution Solution { get; set; }

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
