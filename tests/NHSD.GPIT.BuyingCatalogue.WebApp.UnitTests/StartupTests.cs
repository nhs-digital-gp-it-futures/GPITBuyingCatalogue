using System;
using System.IO;
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
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class StartupTests
    {
        [SetUp]
        public static void SetUp()
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

        [TestCase(typeof(DisabledErrorMessageSettings))]
        [TestCase(typeof(GPITBuyingCatalogueDbContext))]
        [TestCase(typeof(DataProtectorTokenProvider<AspNetUser>))]
        [TestCase(typeof(SmtpSettings))]        
        public static void ContainsTheExpectedServiceInstances(Type expectedType)
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<StartupTest>().Build();

            webHost.Services.GetRequiredService(expectedType).Should().NotBeNull();
        }

        [TestCase(typeof(IEmailService), typeof(MailKitEmailService))]
        [TestCase(typeof(IMailTransport), typeof(SmtpClient))]
        [TestCase(typeof(IMemberValueResolver<object, object, string, string>), typeof(ConfigSettingResolver))]
        [TestCase(typeof(IMemberValueResolver<object, object, string, bool?>), typeof(StringToNullableBoolResolver))]
        [TestCase(typeof(IPasswordResetCallback), typeof(PasswordResetCallback))]
        [TestCase(typeof(IPasswordService), typeof(PasswordService))]
        [TestCase(typeof(IPasswordValidator<AspNetUser>), typeof(PasswordValidator))]
        [TestCase(
            typeof(ITypeConverter<EntityFramework.Models.GPITBuyingCatalogue.CatalogueItem, SolutionStatusModel>),
            typeof(CatalogueItemToSolutionStatusModelConverter))]
        [TestCase(typeof(ITypeConverter<string, bool?>), typeof(StringToNullableBoolResolver))]
        [TestCase(typeof(IUserClaimsPrincipalFactory<AspNetUser>), typeof(UserClaimsPrincipalFactoryEx<AspNetUser>))]
        public static void ContainsTheExpectedServiceInstances(Type requiredInterface, Type expectedType)
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<StartupTest>().Build();

            webHost.Services.GetRequiredService(requiredInterface).Should().BeOfType(expectedType);
        }
    }

    public class StartupTest : Startup
    {
        public StartupTest(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
