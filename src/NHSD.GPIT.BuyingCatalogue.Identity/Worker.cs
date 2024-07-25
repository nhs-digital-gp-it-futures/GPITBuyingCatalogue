using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.Identity.Data;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace NHSD.GPIT.BuyingCatalogue.Identity;

public class Worker(IServiceProvider serviceProvider) : IHostedService
{
    private const string ClientId = "BuyingCatalogueTest";
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync(ClientId, cancellationToken) is not null) return;

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = ClientId,
            ClientSecret = "test-data-secret",
            ConsentType = ConsentTypes.Implicit,
            DisplayName = "Buying Catalogue Login",
            ClientType = ClientTypes.Confidential,
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:5001/signout-callback-oidc"),
            },
            RedirectUris =
            {
                new Uri("https://localhost:5001/signin-oidc"),
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange,
            },
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
