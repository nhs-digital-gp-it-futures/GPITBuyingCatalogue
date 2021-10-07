using System;
using FluentAssertions;
using FluentValidation;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests
{
    public sealed class StartupTests
    {
        public StartupTests()
        {
            Environment.SetEnvironmentVariable(
                "BC_DB_CONNECTION",
                "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BC_Catalog;Trusted_Connection=True;");
            Environment.SetEnvironmentVariable("BC_SMTP_HOST", "localhost");
            Environment.SetEnvironmentVariable("BC_SMTP_PORT", "1081");
            Environment.SetEnvironmentVariable("DOMAIN_NAME", "localhost");
        }

        [Theory]
        [InlineData(typeof(DisabledErrorMessageSettings))]
        [InlineData(typeof(BuyingCatalogueDbContext))]
        [InlineData(typeof(DataProtectorTokenProvider<AspNetUser>))]
        [InlineData(typeof(SmtpSettings))]
        public void ContainsTheExpectedServiceInstances_A(Type expectedType)
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<StartupTest>().Build();

            webHost.Services.GetRequiredService(expectedType).Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(IEmailService), typeof(MailKitEmailService))]
        [InlineData(typeof(IMailTransport), typeof(SmtpClient))]
        [InlineData(typeof(IPasswordResetCallback), typeof(PasswordResetCallback))]
        [InlineData(typeof(IPasswordService), typeof(PasswordService))]
        [InlineData(typeof(IPasswordValidator<AspNetUser>), typeof(PasswordValidator))]
        [InlineData(typeof(IUserClaimsPrincipalFactory<AspNetUser>), typeof(UserClaimsPrincipalFactoryEx))]
        [InlineData(typeof(IValidator<SolutionModel>), typeof(SolutionModelValidator))]
        public void ContainsTheExpectedServiceInstances_B(Type requiredInterface, Type expectedType)
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<StartupTest>().Build();

            webHost.Services.GetRequiredService(requiredInterface).Should().BeOfType(expectedType);
        }
    }
}
