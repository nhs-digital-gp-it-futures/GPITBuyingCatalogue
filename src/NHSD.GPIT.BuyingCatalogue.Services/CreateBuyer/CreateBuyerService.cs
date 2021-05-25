using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
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
        private readonly ILogWrapper<CreateBuyerService> logger;
        private readonly IDbRepository<AspNetUser, UsersDbContext> userRepository;
        private readonly IPasswordService passwordService;
        private readonly IPasswordResetCallback passwordResetCallback;
        private readonly IEmailService emailService;
        private readonly RegistrationSettings settings;
        private readonly IAspNetUserValidator aspNetUserValidator;

        public CreateBuyerService(
            ILogWrapper<CreateBuyerService> logger,
            IDbRepository<AspNetUser, UsersDbContext> userRepository,
            IPasswordService passwordService,
            IPasswordResetCallback passwordResetCallback,
            IEmailService emailService,
            RegistrationSettings settings,
            IAspNetUserValidator aspNetUserValidator)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            this.passwordResetCallback = passwordResetCallback ?? throw new ArgumentNullException(nameof(passwordResetCallback));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.aspNetUserValidator = aspNetUserValidator ?? throw new ArgumentNullException(nameof(aspNetUserValidator));
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
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            var validationResult = await aspNetUserValidator.ValidateAsync(aspNetUser);
            if (!validationResult.IsSuccess)
                return Result.Failure<string>(validationResult.Errors);

            userRepository.Add(aspNetUser);

            await userRepository.SaveChangesAsync();

            var token = await passwordService.GeneratePasswordResetTokenAsync(aspNetUser.Email);

            await SendInitialEmailAsync(token);

            return Result.Success(aspNetUser.Id);
        }

        public async Task SendInitialEmailAsync(PasswordResetToken token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var user = token.User;

            await emailService.SendEmailAsync(
                settings.EmailMessage,
                new EmailAddress(user.Email, user.GetDisplayName()),
                passwordResetCallback.GetPasswordResetCallback(token));
        }
    }
}
