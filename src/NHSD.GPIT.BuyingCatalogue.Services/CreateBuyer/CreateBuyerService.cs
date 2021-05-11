using System;
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
        private readonly ILogWrapper<CreateBuyerService> _logger;
        private readonly IUsersDbRepository<AspNetUser> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IPasswordResetCallback _passwordResetCallback;
        private readonly IEmailService _emailService;
        private readonly RegistrationSettings _settings;
        private readonly IAspNetUserValidator _aspNetUserValidator;

        public CreateBuyerService(ILogWrapper<CreateBuyerService> logger,
            IUsersDbRepository<AspNetUser> userRepository,
            IPasswordService passwordService,
            IPasswordResetCallback passwordResetCallback,
            IEmailService emailService,
            RegistrationSettings settings,
            IAspNetUserValidator aspNetUserValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _passwordResetCallback = passwordResetCallback ?? throw new ArgumentNullException(nameof(passwordResetCallback));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _aspNetUserValidator = aspNetUserValidator ?? throw new ArgumentNullException(nameof(aspNetUserValidator));
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

            var validationResult = await _aspNetUserValidator.ValidateAsync(aspNetUser);
            if (!validationResult.IsSuccess)
                return Result.Failure<string>(validationResult.Errors);

            _userRepository.Add(aspNetUser);

            await _userRepository.SaveChangesAsync();
            
            var token = await _passwordService.GeneratePasswordResetTokenAsync(aspNetUser.Email);

            await SendInitialEmailAsync(token);

            return Result.Success(aspNetUser.Id);
        }

        public async Task SendInitialEmailAsync(PasswordResetToken token)
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
}
