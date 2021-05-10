﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrganisationsControllerTests
    { 
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrganisationsController).Should().BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            typeof(OrganisationsController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController(null, Mock.Of<IOrganisationsService>(), Mock.Of<IOdsService>(),
                Mock.Of<ICreateBuyerService>(), Mock.Of<IUsersService>()));
        }

        [Test]
        public static void Constructor_NullOrganisationService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController(Mock.Of<ILogWrapper<OrganisationsController>>(), null, Mock.Of<IOdsService>(),
                Mock.Of<ICreateBuyerService>(), Mock.Of<IUsersService>()));
        }

        [Test]
        public static void Constructor_NullOdsServiceService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController(Mock.Of<ILogWrapper<OrganisationsController>>(), Mock.Of<IOrganisationsService>(), null,
                Mock.Of<ICreateBuyerService>(), Mock.Of<IUsersService>()));
        }

        [Test]
        public static void Constructor_NullCreateBuyerServiceService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController(Mock.Of<ILogWrapper<OrganisationsController>>(), Mock.Of<IOrganisationsService>(), Mock.Of<IOdsService>(),
                null, Mock.Of<IUsersService>()));
        }

        [Test]
        public static void Constructor_NullUsersServiceService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController(Mock.Of<ILogWrapper<OrganisationsController>>(), Mock.Of<IOrganisationsService>(), Mock.Of<IOdsService>(),
                Mock.Of<ICreateBuyerService>(), null));
        }

        [Test]
        public static async Task Get_Index_ReturnsDefaultView()
        {
            var mockOrganisationsService = new Mock<IOrganisationsService>();

            var organisations = new List<Organisation>
            {
                new Organisation { Name = "Name123", OdsCode = "O12"},
                new Organisation { Name = "Name124", OdsCode = "O13"}
            };

            mockOrganisationsService.Setup(x => x.GetAllOrganisations())
                .ReturnsAsync(organisations);
            
            var controller = new OrganisationsController(Mock.Of<ILogWrapper<OrganisationsController>>(), mockOrganisationsService.Object, Mock.Of<IOdsService>(),
                Mock.Of<ICreateBuyerService>(), Mock.Of<IUsersService>());

            var result = await controller.Index();
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }
    }
}
