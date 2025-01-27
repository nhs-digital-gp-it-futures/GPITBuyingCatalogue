using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuyingCatalogueFunction;

public interface IConfigureServices
{
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}
