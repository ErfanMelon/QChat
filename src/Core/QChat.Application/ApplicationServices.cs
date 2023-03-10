using Microsoft.Extensions.DependencyInjection;
using QChat.Application.Interfaces;
using QChat.Domain.Entities;

namespace QChat.Application;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}
