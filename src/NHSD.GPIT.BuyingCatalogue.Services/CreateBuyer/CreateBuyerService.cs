using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;

namespace NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer
{
    public class CreateBuyerService : ICreateBuyerService
    {
        private const int MaximumFirstNameLength = 100;
        private const int MaximumLastNameLength = 100;
        private const int MaximumPhoneNumberLength = 35;
        private const int MaximumEmailLength = 256;

        private static readonly EmailAddressAttribute EmailAddressAttribute = new();

        private readonly ILogWrapper<CreateBuyerService> _logger;
        private readonly IUsersDbRepository<AspNetUser> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IPasswordResetCallback _passwordResetCallback;
        private readonly IEmailService _emailService;
        private readonly RegistrationSettings _settings;

        public CreateBuyerService(ILogWrapper<CreateBuyerService> logger,
            IUsersDbRepository<AspNetUser> userRepository,
            IPasswordService passwordService,
            IPasswordResetCallback passwordResetCallback,
            IEmailService emailService,
            RegistrationSettings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _passwordResetCallback = passwordResetCallback ?? throw new ArgumentNullException(nameof(passwordResetCallback));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<Result<string>> Create(Guid primaryOrganisationId, string firstName, string lastName, string phoneNumber, string emailAddress)
        {
            var aspNetUser = new AspNetUser
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                UserName = emailAddress,
                NormalizedUserName = emailAddress.ToUpper(),
                Email = emailAddress,
                NormalizedEmail = emailAddress.ToUpper(),
                PrimaryOrganisationId = primaryOrganisationId,
                OrganisationFunction = OrganisationFunction.Buyer.DisplayName,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var validationResult = await ValidateAsync(aspNetUser);
            if (!validationResult.IsSuccess)
                return Result.Failure<string>(validationResult.Errors);

            _userRepository.Add(aspNetUser);

            await _userRepository.SaveChangesAsync();
            
            var token = await _passwordService.GeneratePasswordResetTokenAsync(aspNetUser.Email);

            // TODO: discuss exception handling options
            // TODO: consider moving sending e-mail out of process
            // (the current in-process implementation has a significant impact on response time)
            await SendInitialEmailAsync(token);

            return Result.Success(aspNetUser.Id);
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
                var user = (await _userRepository.GetAllAsync(x => x.NormalizedEmail == email.ToUpper())).FirstOrDefault();
                
                if (user is not null &&
                    string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(ApplicationUserErrors.EmailAlreadyExists());
                }
            }
        }

        private async Task SendInitialEmailAsync(PasswordResetToken token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var user = token.User;

            await _emailService.SendEmailAsync(
                _settings.EmailMessage,
                new EmailAddress(user.Email, user.GetDisplayName()),
                _passwordResetCallback.GetPasswordResetCallback(token));
        }
    }

    // MJRTODO - refactor
    public static class ApplicationUserErrors
    {
        public static ErrorDetails FirstNameRequired()
        {
            return new("FirstNameRequired", nameof(AspNetUser.FirstName));
        }

        public static ErrorDetails FirstNameTooLong()
        {
            return new("FirstNameTooLong", nameof(AspNetUser.FirstName));
        }

        public static ErrorDetails LastNameRequired()
        {
            return new("LastNameRequired", nameof(AspNetUser.LastName));
        }

        public static ErrorDetails LastNameTooLong()
        {
            return new("LastNameTooLong", nameof(AspNetUser.LastName));
        }

        public static ErrorDetails PhoneNumberRequired()
        {
            return new("PhoneNumberRequired", nameof(AspNetUser.PhoneNumber));
        }

        public static ErrorDetails PhoneNumberTooLong()
        {
            return new("PhoneNumberTooLong", nameof(AspNetUser.PhoneNumber));
        }

        public static ErrorDetails EmailRequired()
        {
            return new("EmailRequired", "EmailAddress");
        }

        public static ErrorDetails EmailTooLong()
        {
            return new("EmailTooLong", "EmailAddress");
        }

        public static ErrorDetails EmailInvalidFormat()
        {
            return new("EmailInvalidFormat", "EmailAddress");
        }

        public static ErrorDetails EmailAlreadyExists()
        {
            return new("EmailAlreadyExists", "EmailAddress");
        }
    }
}
