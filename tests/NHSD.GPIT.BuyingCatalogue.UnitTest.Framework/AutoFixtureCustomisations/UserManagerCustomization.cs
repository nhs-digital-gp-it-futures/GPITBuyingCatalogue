using System;
using System.Linq;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

[ExcludesAutoCustomization]
public class UserManagerCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var context = fixture.Create<BuyingCatalogueDbContext>();
        var store = new UserStore<AspNetUser, AspNetRole, BuyingCatalogueDbContext, int, AspNetUserClaim,
            AspNetUserRole, AspNetUserLogin, AspNetUserToken, AspNetRoleClaim>(context);

        var lookupNormalizer = Substitute.For<ILookupNormalizer>();
        lookupNormalizer.NormalizeEmail(Arg.Any<string>())
            .Returns(e => e.Arg<string>().ToUpperInvariant());
        lookupNormalizer.NormalizeName(Arg.Any<string>())
            .Returns(n => n.Arg<string>().ToUpperInvariant());

        fixture.Register<IUserStore<AspNetUser>>(() => store);
        fixture.Register(() => lookupNormalizer);
        fixture.Register(
            () => new UserManager<AspNetUser>(
                store,
                fixture.Freeze<IOptions<IdentityOptions>>(),
                fixture.Freeze<IPasswordHasher<AspNetUser>>(),
                Enumerable.Empty<IUserValidator<AspNetUser>>(),
                Enumerable.Empty<IPasswordValidator<AspNetUser>>(),
                lookupNormalizer,
                fixture.Freeze<IdentityErrorDescriber>(),
                fixture.Freeze<IServiceProvider>(),
                fixture.Freeze<ILogger<UserManager<AspNetUser>>>()));
    }
}
