using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Errors;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;

namespace NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer
{
    public sealed class AspNetUserValidator : IAspNetUserValidator
    {
        private const int MaximumFirstNameLength = 100;
        private const int MaximumLastNameLength = 100;
        private const int MaximumPhoneNumberLength = 35;
        private const int MaximumEmailLength = 256;

        private static readonly EmailAddressAttribute EmailAddressAttribute = new();

        private readonly IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext> usersRepository;

        public AspNetUserValidator(IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext> usersRepository)
        {
            this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task<Result> ValidateAsync(AspNetUser user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            List<ErrorDetails> errors = new List<ErrorDetails>();

            ValidateName(user.FirstName, user.LastName, errors);
            ValidatePhoneNumber(user.PhoneNumber, errors);

            await ValidateEmailAsync(user.Email, errors);

            return errors.Any() ? Result.Failure(errors) : Result.Success();
        }

        private static void ValidateName(
            string firstName,
            string lastName,
            List<ErrorDetails> errors)
        {
            ValidateFirstName(firstName, errors);
            ValidateLastName(lastName, errors);
        }

        private static void ValidateFirstName(string firstName, List<ErrorDetails> errors)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(AspNetUserErrors.FirstNameRequired());
                return;
            }

            if (firstName.Length > MaximumFirstNameLength)
                errors.Add(AspNetUserErrors.FirstNameTooLong());
        }

        private static void ValidateLastName(string lastName, List<ErrorDetails> errors)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(AspNetUserErrors.LastNameRequired());
                return;
            }

            if (lastName.Length > MaximumLastNameLength)
                errors.Add(AspNetUserErrors.LastNameTooLong());
        }

        private static void ValidatePhoneNumber(string phoneNumber, List<ErrorDetails> errors)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add(AspNetUserErrors.PhoneNumberRequired());
                return;
            }

            if (phoneNumber.Length > MaximumPhoneNumberLength)
                errors.Add(AspNetUserErrors.PhoneNumberTooLong());
        }

        private async Task ValidateEmailAsync(string email, List<ErrorDetails> errors)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(AspNetUserErrors.EmailRequired());
                return;
            }

            if (email.Length > MaximumEmailLength)
            {
                errors.Add(AspNetUserErrors.EmailTooLong());
            }
            else if (!EmailAddressAttribute.IsValid(email))
            {
                errors.Add(AspNetUserErrors.EmailInvalidFormat());
            }
            else
            {
                var user = (await usersRepository.GetAllAsync(x => x.NormalizedEmail == email.ToUpper())).FirstOrDefault();

                if (user is not null &&
                    string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(AspNetUserErrors.EmailAlreadyExists());
                }
            }
        }
    }
}
