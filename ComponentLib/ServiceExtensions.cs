using Microsoft.Extensions.DependencyInjection;

namespace ComponentLib;

public static class ServiceExtensions
{
    public static IServiceCollection AddPopupManager(this IServiceCollection services)
    {
        services.AddScoped<DialogService>();

        return services;
    }
}
