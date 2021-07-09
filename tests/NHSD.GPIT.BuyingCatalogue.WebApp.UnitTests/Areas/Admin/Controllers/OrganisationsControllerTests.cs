using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class OrganisationsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrganisationsController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(OrganisationsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
        }

        [Fact]
        public static void Constructor_NullOrganisationService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController( null, Mock.Of<IOdsService>(),
                Mock.Of<ICreateBuyerService>(), Mock.Of<IUsersService>()));
        }

        [Fact]
        public static void Constructor_NullOdsServiceService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController( Mock.Of<IOrganisationsService>(), null,
                Mock.Of<ICreateBuyerService>(), Mock.Of<IUsersService>()));
        }

        [Fact]
        public static void Constructor_NullCreateBuyerServiceService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController( Mock.Of<IOrganisationsService>(), Mock.Of<IOdsService>(),
                null, Mock.Of<IUsersService>()));
        }

        [Fact]
        public static void Constructor_NullUsersServiceService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController( Mock.Of<IOrganisationsService>(), Mock.Of<IOdsService>(),
                Mock.Of<ICreateBuyerService>(), null));
        }
    }
}
