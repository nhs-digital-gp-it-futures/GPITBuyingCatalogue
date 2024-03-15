using System;
using System.Collections.Generic;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

[ExcludesAutoCustomization]
public class UserManagerCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var context = fixture.Create<BuyingCatalogueDbContext>();
        var store = new UserStore<AspNetUser, AspNetRole, BuyingCatalogueDbContext, int, AspNetUserClaim,
            AspNetUserRole, AspNetUserLogin, AspNetUserToken, AspNetRoleClaim>(context);

        var lookupNormalizer = new Mock<ILookupNormalizer>();
        lookupNormalizer.Setup(s => s.NormalizeEmail(It.IsAny<string>()))
            .Returns<string>(e => e.ToUpperInvariant());
        lookupNormalizer.Setup(s => s.NormalizeName(It.IsAny<string>()))
            .Returns<string>(n => n.ToUpperInvariant());

        fixture.Register<IUserStore<AspNetUser>>(() => store);
        fixture.Register(() => lookupNormalizer.Object);
        fixture.Register(
            () => new UserManager<AspNetUser>(
                store,
                fixture.Freeze<IOptions<IdentityOptions>>(),
                fixture.Freeze<IPasswordHasher<AspNetUser>>(),
                fixture.Freeze<IEnumerable<IUserValidator<AspNetUser>>>(),
                fixture.Freeze<IEnumerable<IPasswordValidator<AspNetUser>>>(),
                lookupNormalizer.Object,
                fixture.Freeze<IdentityErrorDescriber>(),
                fixture.Freeze<IServiceProvider>(),
                fixture.Freeze<ILogger<UserManager<AspNetUser>>>()));
    }
}
