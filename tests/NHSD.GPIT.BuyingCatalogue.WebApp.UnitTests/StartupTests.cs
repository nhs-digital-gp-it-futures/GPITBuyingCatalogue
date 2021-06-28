using System;
using AutoMapper;
using FluentAssertions;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests
{
    public sealed class StartupTests
    {
        public StartupTests()
        {
            Environment.SetEnvironmentVariable("BC_DB_CONNECTION",
                "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BC_Catalog;Trusted_Connection=True;");
            Environment.SetEnvironmentVariable("CO_DB_CONNECTION",
                "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=ID_Catalog;Trusted_Connection=True;");
            Environment.SetEnvironmentVariable("BC_BLOB_CONNECTION",
                "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://localhost:10100/devstoreaccount1;QueueEndpoint=http://localhost:10101/devstoreaccount1;TableEndpoint=http://localhost:10102/devstoreaccount1;");
            Environment.SetEnvironmentVariable("BC_BLOB_CONTAINER", "buyingcatalogue-documents");
            Environment.SetEnvironmentVariable("BC_SMTP_HOST", "localhost");
            Environment.SetEnvironmentVariable("BC_SMTP_PORT", "1081");
        }

        [Theory]
        [InlineData(typeof(DisabledErrorMessageSettings))]
        [InlineData(typeof(GPITBuyingCatalogueDbContext))]
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
        [InlineData(typeof(IMemberValueResolver<object, object, string, string>), typeof(ConfigSettingResolver))]
        [InlineData(typeof(IMemberValueResolver<object, object, string, bool?>), typeof(StringToNullableBoolResolver))]
        [InlineData(typeof(IPasswordResetCallback), typeof(PasswordResetCallback))]
        [InlineData(typeof(IPasswordService), typeof(PasswordService))]
        [InlineData(typeof(IPasswordValidator<AspNetUser>), typeof(PasswordValidator))]
        [InlineData(
            typeof(ITypeConverter<CatalogueItem, SolutionStatusModel>),
            typeof(CatalogueItemToSolutionStatusModelConverter))]
        [InlineData(typeof(ITypeConverter<string, bool?>), typeof(StringToNullableBoolResolver))]
        [InlineData(typeof(IUserClaimsPrincipalFactory<AspNetUser>), typeof(UserClaimsPrincipalFactoryEx<AspNetUser>))]
        public void ContainsTheExpectedServiceInstances_B(Type requiredInterface, Type expectedType)
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<StartupTest>().Build();

            webHost.Services.GetRequiredService(requiredInterface).Should().BeOfType(expectedType);
        }
    }

    public sealed class StartupTest : Startup
    {
        public StartupTest(IConfiguration configuration)
            : base(configuration)
        {
        }
    }
}
