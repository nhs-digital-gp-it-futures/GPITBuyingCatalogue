using Microsoft.Extensions.DependencyInjection;

namespace BuyingCatalogueFunction;

public interface IConfigureServices
{
    void ConfigureServices(IServiceCollection serviceCollection);
}
