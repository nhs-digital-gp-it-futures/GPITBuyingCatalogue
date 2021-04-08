using System;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    public sealed class ApplicationUser : IdentityUser
    {
        private ApplicationUser(
            string userName,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            OrganisationFunction organisationFunction,
            Guid primaryOrganisationId)
        {
            if (userName is null)
                throw new ArgumentNullException(nameof(userName));

            if (firstName is null)
                throw new ArgumentNullException(nameof(firstName));

            if (lastName is null)
                throw new ArgumentNullException(nameof(lastName));

            if (email is null)
                throw new ArgumentNullException(nameof(email));

            UserName = userName.Trim();
            NormalizedUserName = UserName.ToUpperInvariant();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PhoneNumber = phoneNumber?.Trim() ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = email.Trim();
            NormalizedEmail = Email.ToUpperInvariant();
            OrganisationFunction = organisationFunction ?? throw new ArgumentNullException(nameof(organisationFunction));
            PrimaryOrganisationId = primaryOrganisationId;
            Disabled = false;
            CatalogueAgreementSigned = false;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string DisplayName => $"{FirstName} {LastName}";

        public Guid PrimaryOrganisationId { get; }

        public OrganisationFunction OrganisationFunction { get; }

        public bool Disabled { get; private set; }

        public bool CatalogueAgreementSigned { get; private set; }

        public static ApplicationUser CreateBuyer(
            string userName,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            Guid primaryOrganisationId)
        {
            return new(
                userName,
                firstName,
                lastName,
                phoneNumber,
                email,
                OrganisationFunction.Buyer,
                primaryOrganisationId);
        }

        public static ApplicationUser CreateAuthority(
            string userName,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            Guid primaryOrganisationId)
        {
            return new(
                userName,
                firstName,
                lastName,
                phoneNumber,
                email,
                OrganisationFunction.Authority,
                primaryOrganisationId);
        }

        public void MarkAsDisabled()
        {
            Disabled = true;
        }

        public void MarkAsEnabled()
        {
            Disabled = false;
        }

        public void MarkCatalogueAgreementAsSigned()
        {
            CatalogueAgreementSigned = true;
        }
    }
}
