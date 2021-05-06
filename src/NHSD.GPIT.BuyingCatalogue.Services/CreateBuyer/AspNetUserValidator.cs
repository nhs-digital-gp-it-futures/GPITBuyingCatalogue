using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
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

        private readonly IUsersDbRepository<AspNetUser> _usersRepository;

        public AspNetUserValidator(IUsersDbRepository<AspNetUser> usersRepository)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
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
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(ApplicationUserErrors.FirstNameRequired());
                return;
            }

            if (firstName.Length > MaximumFirstNameLength)
                errors.Add(ApplicationUserErrors.FirstNameTooLong());
        }

        private static void ValidateLastName(string lastName, List<ErrorDetails> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(ApplicationUserErrors.LastNameRequired());
                return;
            }

            if (lastName.Length > MaximumLastNameLength)
                errors.Add(ApplicationUserErrors.LastNameTooLong());
        }

        private static void ValidatePhoneNumber(string phoneNumber, List<ErrorDetails> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add(ApplicationUserErrors.PhoneNumberRequired());
                return;
            }

            if (phoneNumber.Length > MaximumPhoneNumberLength)
                errors.Add(ApplicationUserErrors.PhoneNumberTooLong());
        }

        private async Task ValidateEmailAsync(string email, List<ErrorDetails> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(ApplicationUserErrors.EmailRequired());
                return;
            }

            if (email.Length > MaximumEmailLength)
            {
                errors.Add(ApplicationUserErrors.EmailTooLong());
            }
            else if (!EmailAddressAttribute.IsValid(email))
            {
                errors.Add(ApplicationUserErrors.EmailInvalidFormat());
            }
            else
            {
                var user = (await _usersRepository.GetAllAsync(x => x.NormalizedEmail == email.ToUpper())).FirstOrDefault();

                if (user is not null &&
                    string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(ApplicationUserErrors.EmailAlreadyExists());
                }
            }
        }
    }
}
